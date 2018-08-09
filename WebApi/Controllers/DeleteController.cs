using Common;
using DeleteSQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class DeleteController : ApiController
    {
        private static string application = string.Empty;
        private static string date = string.Empty;
        private static string userName = string.Empty;
        private static string conString = $@"data source=10.91.56.201;initial catalog=Admin_Altec;user id=altec_store;password=hZB7m52s;MultipleActiveResultSets=True";
        //private static string conString = $@"data source=chunli.alter-frame.fr;initial catalog=Admin_Altec;user id=altec_store;password=hZB7m52s;MultipleActiveResultSets=True";

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public string Get(int id)
        {
            return "value";
        }

        public bool DeletingSharedStudies(Object entity)
        {
            try
            {
                var jsonString = entity.ToString();
                var retJson = JsonConvert.DeserializeObject<SharedStudy>(jsonString);

                var study = new SharedStudy()
                {
                    ApplicationName = retJson.ApplicationName,
                    CreatedDate = retJson.CreatedDate,
                    UserName = retJson.UserName
                };
                
                var userId = GetUserByName(study.UserName, conString);
                var etudeId = GetStudyByDate(study.CreatedDate, study.ApplicationName, conString);

                return DeletingOnRemoteDB(userId, etudeId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deleting on ServerDB
        /// </summary>
        public static bool DeletingOnRemoteDB(int userId, int etudeId)
        {
            if (userId == 0 || etudeId == 0)
            {
                LoggerHelper.Write("Error while getting userId or studyId on remoteDB");
                return false;
            }
            else
                return DeleteSqlValue(userId, etudeId, 3, conString);            
        }

        /// <summary>
        /// Get user id by username
        /// </summary>
        /// <param name="userName">user name</param>
        private static int GetUserByName(string userName, string connect)
        {
            if (string.IsNullOrEmpty(userName))
                return 0;

            var result = 0;
            var sqlRequest = $@"USE [Admin_Altec]; SELECT [Id] FROM [dbo].[ObjUser] WHERE [Login] = '{userName}'";
            try
            {
                using (SqlConnection con = new SqlConnection(connect.ToString()))
                {
                    try
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(sqlRequest, con);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Convert.ToInt32(reader["Id"]);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerHelper.Write("Error while getting user", e);
                        return result;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }

        /// <summary>
        /// Get study id by application name and created date
        /// </summary>
        /// <param name="date">Created date of study</param>
        /// <param name="application">Application name</param>
        private static int GetStudyByDate(string date, string application, string connect)
        {
            var result = 0;
            var sqlRequest = $@"USE [Admin_Altec]; SELECT [Id] FROM [dbo].[ObjEtude] WHERE CAST([CreatedDate] AS DATETIME2) LIKE '{date}%' AND [IdEtude] LIKE '{application}%'";

            try
            {
                using (SqlConnection con = new SqlConnection(connect.ToString()))
                {
                    try
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand(sqlRequest, con);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Convert.ToInt32(reader["Id"]);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerHelper.Write("Error while getting study", e);
                        return result;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }

        /// <summary>
        /// Deleting Shared Study 
        /// </summary>
        /// <param name="idUser">idUser who received the study</param>
        /// <param name="idEture">idEtude who shared</param>
        /// <param name="typeId">Type 3 = Someone who received the demands</param>
        private static bool DeleteSqlValue(int idUser, int idEture, int typeId, string connect)
        {
            // Delete value SQL request
            var sqlRequest = $@"USE [Admin_Altec]; DELETE FROM [dbo].[ObjEtudeShared] WHERE [User_Id] = '{idUser}' AND [Etude_Id] = '{idEture}' AND [Type_Id] = '{typeId}'";
            var result = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connect.ToString()))
                {
                    con.Open();
                    try
                    {
                        SqlCommand cmd = new SqlCommand(sqlRequest, con);
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Action not completed", e);
                    }
                    con.Close();

                    if (result > 0)
                        return true;
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggerHelper.Write("Action not completed", e);
                return false;
            }
        }
    }
}
