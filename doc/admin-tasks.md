# Admin Tasks / Devops

This page rpovides afew examples of admin/devops related commands.

## Ascon

Use `ascon` tool to establish direct admin connection to the process via TCP (no webserver needed).

```bash
# if you do not supply password inline, the tool will prompt you for it
$ ascon myserver1:7797 -c id=root pwd="your-password"
```

## Manc

Get a list of components
```
  manc
```

Get a list of commands that component understands
```
  manc{ sid=123 call-list{}}
```


## Dctl

Dctl allows to control daemons.
Use `manc` to get desired component SID
```
> dctl{sid=123 action=stop};
> dctl{sid=123 action=start};
```

## Mongo Db management
Mongo db connector understands `directdb` component command. It is used for 
general mongo management and usefull for Chronicle and Minidp management as well.

Assuming that `manc` returned `sid=123` for "Connector.MongoClient"

### Get Chronicle Record Count
Connect to `memoir` admin console

Then run:
```
manc
{
  sid=123 // use manc to lookup your component sid
  call
  {
    directdb
    {
      s='mongo://localhost:27017'
      d='sky_chron'
      cmd='{count: "sky_log"}'
    }
  }
}
```

### Fetching records
Connect to `memoir` admin console

Then run:
```
manc
{
  sid=123 // use manc to lookup your component sid
  call
  {
    directdb
    {
      s='mongo://localhost:27017'
      d='sky_chron'
      cmd='{find: "sky_log", limit: 3}' //build query here
    }
  }
}
```

### Create indexes
Connect to `memoir` admin console

You need to create Chronicle indexes in mongo so queries do not take long time.

Refer to this:
  https://docs.mongodb.com/manual/reference/command/createIndexes/

The following exmaple creates an ascending non-unique index on chronicle field `u`:
```
manc
{
  sid=123 // use manc to lookup your component sid
  call
  {
    directdb
    {
      s='mongo://localhost:27017'
      d='sky_chron'
      cmd=$'{createIndexes: "sky_log", indexes: [
       {
         key: {u: 1}, //field name 'u' for utc time stamp
         name: "idx_utc",
         unique: false       
       }
      ]}'
    }
  }
}
```

### Delete old Chronicle log data
Connect to `memoir` admin console

The delete query would work well when `u` field is indexed as described above.

```
manc
{
  sid=123 // use manc to lookup your component sid
  call
  {
    directdb
    {
      s='mongo://localhost:27017'
      d='sky_chron'
      cmd=$'{
        delete: "sky_log", 
        deletes: [
         {
          limit: 1,//this can be either zero or 1 to delete a single document
          q: {u: {"$lt": "mm/dd/yyyy"}}
         } 
        ]}'
    }
  }
}
```


## Bash curl loop through files
```bash
#!/bin/bash

for file in /this/is/my/path/example*
do
  curl -X POST -T "/this/is/my/path/$file"  https://whatever
done; 
```



