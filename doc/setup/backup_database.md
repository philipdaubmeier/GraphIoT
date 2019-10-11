# Backup database

On the server, you can back up your production database by doing the following steps:

Run with sqlcmd.exe (run `cmd` and start it with `sqlcmd -S .\SQLEXPRESS`):

```sql
BACKUP DATABASE [GraphIoT] TO DISK='C:\bkp\graphiot_db.bak';
GO
```

And to restore:

```sql
RESTORE DATABASE [GraphIoT] FROM DISK='C:\bkp\graphiot_db.bak';
GO
```
