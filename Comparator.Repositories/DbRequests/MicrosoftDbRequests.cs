namespace Comparator.Repositories.DbRequests
{
    internal class MicrosoftDbRequests
    {
        public string GetTablesRequest(string ownerName) =>
            $"EXEC sp_tables @table_owner = '{ownerName}'";

        public string GetFieldsRequest(string tableName) =>
            $"EXEC sp_columns @table_name = '{tableName}'";

        public string GetConstraintsRequest(string tableName) =>
            $"EXEC sp_helpconstraint {tableName}, nomsg";

        public string GetProceduresRequest(string ownerName) =>
            $"EXEC sp_stored_procedures @sp_owner = '{ownerName}'";

        public string GetSqriptRequest(string procedureName) =>
            $"EXEC sp_helptext {procedureName}";   
                
        public string GetTreggersRequest() =>
            $"SELECT name FROM sys.triggers";
    }
}

// EXEC sp_helptext Customers_Add_Update - скрипт 
// EXEC sp_depends Customers_Add_Update - инфа
// EXEC sp_helptrigger Customers - все триггеры в таблице





//CREATE TRIGGER Customers_Add_Update
//ON Customers
//AFTER INSERT, UPDATE
//AS
//UPDATE Customers
//SET Age = 'Update'
//WHERE Phone = (SELECT Id FROM inserted)



//    CREATE TRIGGER Customers_Before
//ON Customers
//BEFORE INSERT, UPDATE
//AS
//UPDATE Customers
//SET Age = 'Update'
//WHERE Phone = (SELECT Id FROM inserted)





    
//CREATE TRIGGER "Customers_Add_Update" AFTER INSERT, UPDATE ON "Customers" referencing old
//    AS "current_Customers" 
//    SET FirstName = "qwe"
//WHERE Phone = (SELECT Id FROM inserted)


//create trigger "TR_check" before delete on
//"root"."Customers"
//referencing old as "current_Customers"
//for each row when("current_Customers"."Age" is null)
//begin
//  raiserror 30001 'You cannot delete an employee who has not been fired'
//end