# О программе 
TransferData программа для переноса данных тоблицы из одной СУБД в другую  
На данный момент поддерживаются следующие СУБД: PostgreSQL, SQL Server  

# Начало работы 
Для начала работы с программой необходимо настроить appsettings.json, указав ваш тип СУБД и настройки чтобы программа могла подключиться к вашей базе данных  
Параметр AppDbOptions:DbType отвечает за тип вашей СУБД  
0 - PostgreSQL  
1 - SQL Server

Параметр ConnectionStrings:DataContext отвечает за строку подключение к вашей базе данных  

# Работа 
Это консольное приложение которое принимает в качестве аргументов 2 обязательных параметра  
-t название таблицы с которой нужно перенести данные  
-d название СУБД в которую нужно перенести данные таблицы  
Пример вызова из командной строки  
```console
TransferData.ConsoleView.exe -t table1 -d MSSQL
```

# Результат работы  
Если программа успешно завершает свою работу, на рабочем столе появится файл содержащий 2 SQL команды  

Команда для создания временной таблицы
```sql
select id1, realnum, id2 into #Temptable1 from
(
    select 1 as id1, 625.67 as realnum, 2 as id2 union all
    select 2 as id1, 33.67 as realnum, 3 as id2 union all
    select 3 as id1, 13.67 as realnum, 1 as id2 union all
    select 4 as id1, 612.67 as realnum, 2 as id2 union all
    select 5 as id1, 14.2 as realnum, 2 as id2 union all
    select 6 as id1, 555.5 as realnum, 3 as id2
) as dt
```

Команда для синхронизации таблиц  
```sql
merge table1 AS T_Base 
using #Temptable1 AS T_Source 
on (T_Base.id1 = T_Source.id1) 
when matched then 
update set realnum = T_Source.realnum, id2 = T_Source.id2 
when not matched then 
insert (id1, realnum, id2) 
values (T_Source.realnum, T_Source.id2) 
--when not matched by source then delete
```
