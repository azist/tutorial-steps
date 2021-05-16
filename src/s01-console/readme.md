# Step 1 - CLI (Command Line Interface)

We are going to build a CLI - [c]ommand [l]ine [i]nterface app which takes input from
the command line arguments. 

> We strongly recommend that you review 
[Project Structure Recap](/doc/project-structure.md) first


We start at the entry point [`Program.cs`](Program.cs) method `int Main(string[] args)` which
takes command line arguments as supplied by the operating system, and returns a code back to the operating system.
Non-zero return values typically denote various error conditions. See [Main() return values](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/main-and-command-args/main-return-values)

The example demonstrates a typical pattern with `try/catch` - if something happens in the program body
we will show exception name and return `-1` instead of crashing the process. The actual work is delegated into 
`main(app)` call. Many things can go wrong, for example:
* An attempt to mount application chassis `new AzosApplication()` may fail because configuration file may be invalid
* Some unhandled error may happen in `main()`

The application container/chassis initializes all services (such as DI, config, logging etc..) as
prescribed in the configuration file. The application config file is taken from the same path where executable is
the project includes a file `*.laconf` which has its build action set to "Copy to Output". This way, the config file
is copied by the build process automatically. Unlike complex legacy "App.config"-based solution provided by legacy .NET
framework, Azos config files are not processed by any tool and copied to output as-is which significantly simplifies
configuration aspect, however Azos configuration format is much more powerful (e.g. support for variables, macros) which
makes DevOps tasks simpler. We will look at Azos configuration in the next tutorial steps.

The command line args get parsed by app chassis and exposed as another configuration object called `IApplication.CommandArgs`.
This design allows for unification of configuration techniques, as this example shows. We can configure
classes from any source, be it app config or command line args.

We illustrate how to query command line args via config navigation e.g. 
```csharp
  //The [] accessor coalesces sub-section names from left to right
  //until a section with such name is found. If nothing found then
  //a special "Nonexistent" sentinel node section is returned
  var isSilent = app.CommandArgs["s", "silent"].Exists; // `-s` or `-silent` switch in the root
```

We then dump app information into console. Pay attention to the line which prints command args back to the user.
The info dump is performed unless `silent` switch is present. It is a typical console app design to suppress "logo"
and other verbose messages. In Unix-like operating systems many system programs (e.g. `awk`, `grep`, `sed` etc.) are 
"piped" into each other forming complex CLI processing conveyors. A "silent" switch may be needed to suppress
info messages which would have prevented the possibility of utility conveyor composition.

The next step illustrates command args-driven dependency injection.
This pattern is very much needed in many complex command-line tools, e.g. in compilers and transformations where
the actual work is not hard-coded but rather delegated into another entity.
We have abstracted a unit of some work using a `Work` abstraction. See [Work.cs](Work.cs).
The class serves as a base with abstract `Perform()` method. It is a `IConfigurable`-implementor: it applies
config options from the supplied config section into its object instance. We then allocate and configure a specific instance of the
`Work`-derived class with one line:
```csharp
 //Polymorphism: inject dependency using `type=FQN` syntax
 var work = FactoryUtils.MakeAndConfigure<Work>(workConf, typeof(DefaultWork));//if `type` was not supplied, use `DefaultWork
```
And then we call the method that does actual work:
```csharp
 //Polymorphism: what is done depends on the actual type of work object
 var result = work.Perform(); //<---- PERFORM ACTUAL WORK
```

This works because your command-line args got parsed into configuration object `app.CommandArgs`, here is an example:

```csharp
//Command line
//$ dotnet s01-console.dll -silent -work type="s01_console.LoopWork, s01-console" from=5 to=23 
//was parsed into a config tree:
args
{
  silent{ }
  work
  {
    type="s01_console.LoopWork, s01-console"
    from=5
    to=23
  }
}
```

The aforementioned call to `FactoryUtils` interprets the `type` attribute of the `work` section
and tries to inject a CLR type. If the attribute is not specified, then `typeof(DefaultWork)` will
be used by default, consequently you can abridge the call args for `DefaultWork`:
```
$ dotnet s01-console.dll
... Message is: Default message text...
```

Notice how the classes derived from `Work` can consume their properties from config object.
```csharp
  [Config(Default = 1)]
  public int From { get; set; }
```
The system would bind the value automatically be it app config or command lines arguments.

As illustrated above, the framework performs a lot of boilerplate plumbing. App chassis provides logical container isolation 
of your app instance which is the application composition root.

