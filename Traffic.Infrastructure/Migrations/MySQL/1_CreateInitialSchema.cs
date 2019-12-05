using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Traffic.Infrastructure.Migrations.MySQL
{
    [Migration(1)]
    public class CreateInitialSchema : Migration
    {
        public override void Up()
        {
            Execute.EmbeddedScript("1_CreateInitialSchema.sql");
        }

        public override void Down()
        {
        }
    }
}
