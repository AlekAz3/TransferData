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
Если программа успешно завершает свою работу, на рабочем столе появится файл содержащий SQL запросы для переноса данных, и если у исходной таблицы есть внешние ключи то программа создаст запросы для синхрацизации всех таблиц от которых зависит главная таблица 

Команда для создания временной таблицы 
```sql
select [id1], [field1], [id2], [id4], [boolf] into #TempTable1 from
( 
select 1 as [id1], 'table1' as [field1], 1 as [id2], 1 as [id4], 'True' as [boolf] union all
select 2 as [id1], 'table1' as [field1], 1 as [id2], 1 as [id4], 'True' as [boolf] union all
select 3 as [id1], 'table3' as [field1], 3 as [id2], 1 as [id4], 'True' as [boolf] union all
select 4 as [id1], 'table4' as [field1], 4 as [id2], 1 as [id4], 'True' as [boolf] union all
select 5 as [id1], 'table5' as [field1], 5 as [id2], 1 as [id4], 'True' as [boolf] union all
select 6 as [id1], 'table6' as [field1], 6 as [id2], 1 as [id4], 'True' as [boolf] union all
select 7 as [id1], 'table2' as [field1], 7 as [id2], 1 as [id4], 'False' as [boolf]
) as dt;
```

Команда для синхронизации таблиц  
```sql
merge Table1 AS T_Base 
using #TempTable1 AS T_Source 
on (T_Base.[id1] = T_Source.[id1]) 
when matched then 
update set [id1] = T_Source.[id1], [field1] = T_Source.[field1], [id2] = T_Source.[id2], [id4] = T_Source.[id4], [boolf] = T_Source.[boolf] 
when not matched then 
insert ([id1], [field1], [id2], [id4], [boolf]) 
values (T_Source.[id1], T_Source.[field1], T_Source.[id2], T_Source.[id4], T_Source.[boolf]) 
;--when not matched by source then delete;
```
