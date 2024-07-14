import { readFile } from "fs/promises";
import path from "path";

const dropSqlFile = path.resolve(import.meta.dirname, "drop.sql");
const schemaSqlFile = path.resolve(import.meta.dirname, "schema.sql");

const dropSql = await readFile(dropSqlFile, "utf-8");
const schemaSql = await readFile(schemaSqlFile, "utf-8");

const libSqlUrl = process.env.LIBSQL_CLIENT_URL;
const libSqlToken = process.env.LIBSQL_CLIENT_TOKEN;

const request = {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
    "Authorization": libSqlToken ? `Bearer ${libSqlToken}` : undefined,
  },
  body: JSON.stringify({
    requests: [
      {
        type: "execute",
        stmt: {
          sql:dropSql,
        }
      },
      {
        type: "execute",
        stmt: {
          sql:schemaSql,
        }
      },
      {
        type: "close"
      }
    ]
  })
};

console.log(`Setting up database ${libSqlUrl.replace(/https?:\/\//, "")}...`);

var res = await fetch(`${libSqlUrl}/v3/pipeline`, request)

if (!res.ok) {
  throw new Error(`Failed to setup database: ${res.statusText}`);
}

console.log("Database setup complete");
