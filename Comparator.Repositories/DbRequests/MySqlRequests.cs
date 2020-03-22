namespace Comparator.Repositories.DbRequests
{
    public class MySqlRequests
    {
        public string GetTablesRequest(string dbName) =>
            $"SHOW TABLES FROM {dbName}";

        public string GetFieldsRequest(string dbName, string tableName) => 
            $"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT, EXTRA, ORDINAL_POSITION," +
            $"CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' " +
            $"AND TABLE_SCHEMA = '{dbName}' ORDER BY ORDINAL_POSITION;";

        public string GetConstraintsRequest(string dbName, string tableName) =>
            $"SELECT DISTINCT i.CONSTRAINT_TYPE, i.CONSTRAINT_NAME, k.COLUMN_NAME, k.REFERENCED_TABLE_NAME, " +
            $"k.REFERENCED_COLUMN_NAME FROM information_schema.TABLE_CONSTRAINTS i LEFT JOIN " +
            $"information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME WHERE " +
            $"i.TABLE_NAME = '{tableName}' AND k.TABLE_NAME = '{tableName}' AND i.TABLE_SCHEMA = '{dbName}';";

        public string GetCascadeOptions(string dbName, string tableName) => 
            $"SELECT CONSTRAINT_NAME, UPDATE_RULE, DELETE_RULE FROM " +
            $"information_schema.REFERENTIAL_CONSTRAINTS WHERE TABLE_NAME = '{tableName}'  AND CONSTRAINT_SCHEMA = '{dbName}'";

        public string GetProceduresRequest(string dbName) =>
            $"SHOW PROCEDURE STATUS WHERE Db = '{dbName}'";

        public string GetProcedureSqriptRequest(string procedureName) => 
            $"SHOW CREATE PROCEDURE {procedureName}";

        public string GetTreggersRequest(string dbName) =>
            $"SHOW TRIGGERS FROM {dbName}";

        public string GetTreggersSqriptRequest(string dbName) => 
            $"select action_statement from information_schema.triggers where TRIGGER_NAME = '{ dbName}'";
    }
}


/*
 
    CREATE TABLE `userdb`.`user` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `FirstName` VARCHAR(45) NULL,
  `Age` INT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Id_UNIQUE` (`Id` ASC) VISIBLE);
     
     */

/*

CREATE PROCEDURE GetAll()
BEGIN
SELECT * FROM user;
END

 */

/*

 USE `userdb`;
DROP procedure IF EXISTS `GetAll`;

DELIMITER $$
USE `userdb`$$
CREATE PROCEDURE GetAll()
BEGIN
SELECT * FROM user;
END$$

DELIMITER ;

    DELIMITER 
CREATE TRIGGER `delete_test` before delete ON `test`
FOR EACH ROW BEGIN
  INSERT INTO backup Set row_id = OLD.id, content = OLD.content;
END;




    select action_statement
from information_schema.triggers where TRIGGER_NAME = 'set'

    SELECT * FROM INFORMATION_SCHEMA.ROUTINES





    $"USE INFORMATION_SCHEMA; SELECT TABLE_NAME, COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME, " +
            $"REFERENCED_COLUMN_NAME FROM KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '{dbName}' " +
            $"AND TABLE_NAME = '{tableName}' AND REFERENCED_COLUMN_NAME IS NOT NULL;";


CREATE TABLE `userdb`.`sio` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `gogle` VARCHAR(122) NULL DEFAULT 'qwe',
  `tramp` INT NULL DEFAULT 321,
  `productId` INT NULL,
  `customerId` INT NULL,
  `userId` INT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Id_UNIQUE` (`Id` ASC) VISIBLE,
  UNIQUE INDEX `gogle_UNIQUE` (`gogle` ASC) VISIBLE,
  UNIQUE INDEX `tramp_UNIQUE` (`tramp` ASC) VISIBLE,
  INDEX `customerId_idx` (`customerId` ASC) VISIBLE,
  INDEX `productId_dd_idx` (`productId` ASC) VISIBLE,
  CONSTRAINT `productId_dd`
    FOREIGN KEY (`productId`)
    REFERENCES `userdb`.`user` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `customerId_uDp`
    FOREIGN KEY (`customerId`)
    REFERENCES `userdb`.`customer` (`id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `userId_AS`
    FOREIGN KEY (`userId`)
    REFERENCES `userdb`.`user` (`Id`)
    ON DELETE SET NULL
    ON UPDATE RESTRICT);





 */
