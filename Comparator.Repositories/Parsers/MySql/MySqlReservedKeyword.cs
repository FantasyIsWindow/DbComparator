﻿using System.Collections.Generic;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlReservedKeyword : ReservedKeywordLibrary
    {
        public MySqlReservedKeyword()
        {
            _reservedWords = new HashSet<string>()
            {
                "ADD",
                "ALL",
                "ALTER",
                "ANALYZE",
                "AND",
                "AS",
                "ASC",
                "BEFORE",
                "BEGIN",
                "BETWEEN",
                "BIGINT",
                "BINARY",
                "BLOB",
                "BOTH",
                "BY",
                "CASCADE",
                "CASE",
                "CHANGE",
                "CHAR",
                "CHARACTER",
                "CHECK",
                "COLLATE",
                "COLUMN",
                "COLUMNS",
                "CONSTRAINT",
                "CONVERT",
                "CREATE",
                "CROSS",
                "CURRENT_DATE",
                "CURRENT_TIME",
                "CURRENT_TIMESTAMP",
                "CURRENT_USER",
                "DATABASE",
                "DATABASES",
                "DAY_HOUR",
                "DAY_MICROSECOND",
                "DAY_MINUTE",
                "DAY_SECOND",
                "DEC",
                "DECIMAL",
                "DEFAULT",
                "DELAYED",
                "DELETE",
                "DESC",
                "DESCRIBE",
                "DISTINCT",
                "DISTINCTROW",
                "DIV",
                "DOUBLE",
                "DROP",
                "DUAL",
                "ELSE",
                "ENCLOSED",
                "ESCAPED",
                "EXISTS",
                "EXPLAIN",
                "FALSE",
                "FIELDS",
                "FLOAT",
                "FLOAT4",
                "FLOAT8",
                "FOR",
                "FORCE",
                "FOREIGN",
                "FROM",
                "END",
                "FULLTEXT",
                "GRANT",
                "GROUP",
                "HAVING",
                "HIGH_PRIORITY",
                "HOUR_MICROSECOND",
                "HOUR_MINUTE",
                "HOUR_SECOND",
                "IF",
                "IGNORE",
                "IN",
                "INDEX",
                "INFILE",
                "INNER",
                "INSERT",
                "INT",
                "INT1",
                "INT2",
                "INT3",
                "INT4",
                "INT8",
                "INTEGER",
                "INTERVAL",
                "INTO",
                "IS",
                "JOIN",
                "KEY",
                "KEYS",
                "KILL",
                "LEADING",
                "LEFT",
                "LIKE",
                "LIMIT",
                "LINES",
                "LOAD",
                "LOCALTIME",
                "LOCALTIMESTAMP",
                "LOCK",
                "LONG",
                "LONGBLOB",
                "LONGTEXT",
                "LOW_PRIORITY",
                "MATCH",
                "MEDIUMBLOB",
                "MEDIUMINT",
                "MEDIUMTEXT",
                "MIDDLEINT",
                "MINUTE_MICROSECOND",
                "MINUTE_SECOND",
                "MOD",
                "NATURAL",
                "NOT",
                "NO_WRITE_TO_BINLOG",
                "NULL",
                "NUMERIC",
                "ON",
                "OPTIMIZE",
                "OPTION",
                "OPTIONALLY",
                "OR",
                "ORDER",
                "OUTER",
                "OUTFILE",
                "PRECISION",
                "PRIMARY",
                "PRIVILEGES",
                "PROCEDURE",
                "PURGE",
                "READ",
                "REAL",
                "REFERENCES",
                "REGEXP",
                "RENAME",
                "REPLACE",
                "REQUIRE",
                "RESTRICT",
                "REVOKE",
                "RIGHT",
                "RLIKE",
                "SECOND_MICROSECOND",
                "SELECT",
                "SEPARATOR",
                "SET",
                "SHOW",
                "SMALLINT",
                "SONAME",
                "SPATIAL",
                "SQL_BIG_RESULT",
                "SQL_CALC_FOUND_ROWS",
                "SQL_SMALL_RESULT",
                "SSL",
                "STARTING",
                "STRAIGHT_JOIN",
                "TABLE",
                "TABLES",
                "TERMINATED",
                "THEN",
                "TINYBLOB",
                "TINYINT",
                "TINYTEXT",
                "TO",
                "TRAILING",
                "TRUE",
                "UNION",
                "UNIQUE",
                "UNLOCK",
                "UNSIGNED",
                "UPDATE",
                "USAGE",
                "USE",
                "USING",
                "UTC_DATE",
                "UTC_TIME",
                "UTC_TIMESTAMP",
                "VALUES",
                "VARBINARY",
                "VARCHAR",
                "VARCHARACTER",
                "VARYING",
                "WHEN",
                "WHERE",
                "WITH",
                "WRITE",
                "XOR",
                "YEAR_MONTH",
                "ZEROFILL",
                "CHECK",
                "FORCE",
                "LOCALTIME",
                "LOCALTIMESTAMP",
                "REQUIRE",
                "SQL_CALC_FOUND_ROWS",
                "SSL",
                "XOR",
                "BEFORE",
                "COLLATE",
                "CONVERT",
                "CURRENT_USER",
                "DAY_MICROSECOND",
                "DIV",
                "DUAL",
                "FALSE",
                "HOUR_MICROSECOND",
                "MINUTE_MICROSECOND",
                "MOD",
                "NO_WRITE_TO_BINLOG",
                "SECOND_MICROSECOND",
                "SEPARATOR",
                "SPATIAL",
                "TRUE",
                "UTC_DATE",
                "UTC_TIME",
                "UTC_TIMESTAMP",
                "VARCHARACTER",
                "ASENSITIVE",
                "CALL",
                "CONDITION",
                "CONNECTION",
                "CONTINUE",
                "CURSOR",
                "DECLARE",
                "DETERMINISTIC",
                "EACH",
                "ELSEIF",
                "EXIT",
                "FETCH",
                "GOTO",
                "INOUT",
                "INSENSITIVE",
                "ITERATE",
                "LABEL",
                "LEAVE",
                "LOOP",
                "MODIFIES",
                "OUT",
                "READS",
                "RELEASE",
                "REPEAT",
                "RETURN",
                "SCHEMA",
                "SCHEMAS",
                "SENSITIVE",
                "SPECIFIC",
                "SQL",
                "SQLEXCEPTION",
                "SQLSTATE",
                "SQLWARNING",
                "TRIGGER",
                "UNDO",
                "UPGRADE",
                "WHILE",
                "ACCESSIBLE",
                "LINEAR",
                "MASTER_SSL_VERIFY_SERVER_CERT",
                "RANGE",
                "READ_ONLY",
                "READ_WRITE",
                "GENERAL",
                "IGNORE_SERVER_IDS",
                "MASTER_HEARTBEAT_PERIOD",
                "MAXVALUE",
                "RESIGNAL",
                "SIGNAL",
                "SLOW",
                "GET",
                "IO_AFTER_GTIDS",
                "IO_BEFORE_GTIDS",
                "MASTER_BIND",
                "ONE_SHOT",
                "PARTITION",
                "SQL_AFTER_GTIDS",
                "SQL_BEFORE_GTIDS",
                "NONBLOCKING"
            };

            _tab = new HashSet<string>()
            {
                "SET",
                "CASE",
                "WHEN",
                "ELSE",
                "LEFT",
                "RIGHT",
                "GROUP",
                "ORDER",
                "VALUES",
                "HAVING",
                "DECLARE",
                "WHERE",
                "INNER",
                "INSERT",
                "IF",
                "RETURNS",
                "DELETE",
                "RETURN"
            };

            _newLine = new HashSet<string>()
            {
                "CREATE"
            };
        }
    }
}