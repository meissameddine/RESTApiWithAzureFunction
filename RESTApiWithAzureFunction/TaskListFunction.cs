using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace RESTApiWithAzureFunction
{
    class TaskListFunction
    {

        [FunctionName("GetTasks")]
        public static async Task<IActionResult> GetTasks(
               [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "list")] HttpRequest req, ILogger log)
        {
            List<TaskModel> taskList = new List<TaskModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select * from CovidProof";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        TaskModel task = new TaskModel()
                        {
                           EvaxId = (int)reader["EvaxId"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                          cin = (int)reader["cin"],
                          // tel = (int)reader["tel"]
                        };
                        taskList.Add(task);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (taskList.Count > 0)
            {
                return new OkObjectResult(taskList);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        
        [FunctionName("GetTaskById")]
        public static IActionResult GetTaskById(
     [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "listId/{EvaxId}")] HttpRequest req, ILogger log, int EvaxId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select * from CovidProof Where EvaxId = @EvaxId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EvaxId", EvaxId);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (dt.Rows.Count == 0)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(dt);
        }





        [FunctionName("CreateTask")]
        public static async Task<IActionResult> CreateTask(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add")] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<CreateTaskModel>(requestBody);
            try {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString"))) {
                    connection.Open();
                        var query = $"INSERT INTO [CovidProof] (EvaxId,FirstName,LastName,cin) VALUES('{input.EvaxId}', '{input.FirstName}', '{input.LastName}', '{input.cin}')";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e) {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            return new OkResult();
        }


        [FunctionName("DeleteTask")]
        public static IActionResult DeleteTask(
       [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{EvaxId}")] HttpRequest req, ILogger log, int EvaxId)
        {
            try {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString"))) {
                    connection.Open();
                    var query = @"Delete from CovidProof Where EvaxId = @EvaxId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EvaxId", EvaxId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e) {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            return new OkResult();
        }
    }

}
