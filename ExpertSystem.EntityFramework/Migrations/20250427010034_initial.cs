using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExpertSystem.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:logic_operator", "and,or")
                .Annotation("Npgsql:Enum:metric", "accuracy,f1score,precision,recall")
                .Annotation("Npgsql:Enum:model_type", "knn,linear_regression,bayes,neural_network,own")
                .Annotation("Npgsql:Enum:operator", "greater_than,greater_than_or_equal,less_than,less_than_or_equal")
                .Annotation("Npgsql:Enum:plot_type", "confusion_matrix,roc")
                .Annotation("Npgsql:Enum:set_type", "training_set,validation_set,test_set");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    PasswordHashed = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    DatasetId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.DatasetId);
                    table.ForeignKey(
                        name: "FK_Datasets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiments",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DatasetID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiments", x => x.ExperimentId);
                    table.ForeignKey(
                        name: "FK_Experiments_Datasets_DatasetID",
                        column: x => x.DatasetID,
                        principalTable: "Datasets",
                        principalColumn: "DatasetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Experiments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DecisionRules",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExperimentID = table.Column<int>(type: "integer", nullable: false),
                    Metric = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Threshold = table.Column<double>(type: "double precision", nullable: false),
                    LogicOperator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionRules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_DecisionRules_Experiments_ExperimentID",
                        column: x => x.ExperimentID,
                        principalTable: "Experiments",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelConfigurations",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExperimentId = table.Column<int>(type: "integer", nullable: false),
                    ModelType = table.Column<string>(type: "text", nullable: false),
                    Hyperparameters = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelConfigurations", x => x.ConfigId);
                    table.ForeignKey(
                        name: "FK_ModelConfigurations_Experiments_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiments",
                        principalColumn: "ExperimentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelResults",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConfigId = table.Column<int>(type: "integer", nullable: false),
                    SetType = table.Column<string>(type: "text", nullable: false),
                    Accuracy = table.Column<int>(type: "integer", nullable: false),
                    F1Score = table.Column<int>(type: "integer", nullable: false),
                    Precision = table.Column<int>(type: "integer", nullable: false),
                    Recall = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelResults", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_ModelResults_ModelConfigurations_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "ModelConfigurations",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plots",
                columns: table => new
                {
                    PlotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultId = table.Column<int>(type: "integer", nullable: false),
                    PlotType = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plots", x => x.PlotId);
                    table.ForeignKey(
                        name: "FK_Plots_ModelResults_ResultId",
                        column: x => x.ResultId,
                        principalTable: "ModelResults",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_UserId",
                table: "Datasets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionRules_ExperimentID",
                table: "DecisionRules",
                column: "ExperimentID");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_DatasetID",
                table: "Experiments",
                column: "DatasetID");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_UserId",
                table: "Experiments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelConfigurations_ExperimentId",
                table: "ModelConfigurations",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelResults_ConfigId",
                table: "ModelResults",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Plots_ResultId",
                table: "Plots",
                column: "ResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DecisionRules");

            migrationBuilder.DropTable(
                name: "Plots");

            migrationBuilder.DropTable(
                name: "ModelResults");

            migrationBuilder.DropTable(
                name: "ModelConfigurations");

            migrationBuilder.DropTable(
                name: "Experiments");

            migrationBuilder.DropTable(
                name: "Datasets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
