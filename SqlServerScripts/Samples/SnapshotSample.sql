CREATE DATABASE ContainerEditor_s1 ON  
( NAME = ContainerEditor, FILENAME =   
'D:\ITrash\SqlSnapshot\ContainerEditor_s1.ss' )  
AS SNAPSHOT OF ContainerEditor;  
GO  


insert into ContainerEditor..SrvInnerRefs values ('test ref')
select * from ContainerEditor_s1..SrvInnerRefs
select * from ContainerEditor..SrvInnerRefs


use master
RESTORE DATABASE ContainerEditor from   
DATABASE_SNAPSHOT = 'ContainerEditor_s1';  


IF EXISTS (SELECT  * FROM sys.databases  
    WHERE NAME='ContainerEditor_s1')  
    DROP DATABASE ContainerEditor_s1;  