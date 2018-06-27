using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;
using WebSportingFixtures.Core.Interfaces;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.Services
{
    public class SQLiteStore : IStore
    {
        private readonly string _connectionString;

        public SQLiteStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Team CreateTeam(Team team)
        {
            int id = -1;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "INSERT INTO Teams(Name, KnownName) VALUES (@Name, @KnownName); SELECT last_insert_rowid() as Id";
                    id = dbConnection.QuerySingle<int>(queryString, team);
                }
            }
            catch (SQLiteException)
            {
                return null;
            }

            if (id >= 0)
            {
                team.Id = id;
                return team;
            }
            else
            {
                return null;
            }

        }

        public bool EditTeam(Team team)
        {
            int rowsAffected = 0;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "UPDATE Teams " +
                                         "SET Name=@Name, " +
                                         "KnownName=@KnownName " +
                                         "WHERE Id=@Id";

                    rowsAffected = dbConnection.Execute(queryString, new { team.Name, team.KnownName, team.Id });
                }
            }
            catch (SQLiteException)
            {
                return false;
            }

            return rowsAffected != 0;
        }

        public bool DeleteTeam(int id)
        {
            int rowsAffected = 0;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "PRAGMA foreign_keys=ON; " +
                                         "DELETE FROM Teams WHERE Id=@Id";
                    rowsAffected = dbConnection.Execute(queryString, new { id });
                }
            }
            catch (SQLiteException)
            {
                return false;
            }

            return rowsAffected != 0;
        }

       

        public Team GetTeam(int id)
        {
            
            Team team = null;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "SELECT * FROM Teams " +
                                         "WHERE Id=@Id";
                    team = dbConnection.QueryFirstOrDefault<Team>(queryString, new { id });
                }
            }
            catch (SQLiteException)
            {
                return null;
            }

            return team;
            
        }

        public IEnumerable<Team> GetAllTeams()
        {
            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "SELECT * FROM Teams";
                    return dbConnection.Query<Team>(queryString);
                }
            }
            catch (SQLiteException)
            {
                return new List<Team>();
            }
        }

        public Event CreateEvent(Event anEvent)
        {
            int id = -1;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "INSERT INTO Events(HomeTeam, AwayTeam, Status) " +
                                         "VALUES (@HomeTeamId,@AwayTeamId,@Status); " +
                                         "SELECT last_insert_rowid() as Id";

                    id = dbConnection.QuerySingle<int>(queryString, new { HomeTeamId = anEvent.Home.Id, AwayTeamId = anEvent.Away.Id, anEvent.Status });
                }
            }
            catch (SQLiteException)
            {
                return null;
            }

            if (id >= 0)
            {
                anEvent.Id = id;
                return anEvent;
            }
            else
            {
                return null;
            }
        }

        public bool DeleteEvent(int id)
        {
            int rowsAffected = 0;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "DELETE FROM Events WHERE Id=@Id";
                    rowsAffected = dbConnection.Execute(queryString, new { id });
                }
            }
            catch (SQLiteException)
            {
                return false;
            }

            return rowsAffected != 0;
        }

        public bool EditEvent(Event anEvent)
        {
            int rowsAffected = 0;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "UPDATE Events " +
                                         "SET HomeTeam=@HomeTeamId, " +
                                         "AwayTeam=@AwayTeamId, " +
                                         "Status=@Status " +
                                         "WHERE Id=@Id";
                    rowsAffected = dbConnection.Execute(queryString, new
                    {
                        HomeTeamId = anEvent.Home.Id,
                        AwayTeamId = anEvent.Away.Id,
                        Status = anEvent.Status,
                        Id = anEvent.Id
                    });
                }
 
            }
            catch (SQLiteException)
            {
                return false;
            }

            return rowsAffected != 0;
        }

        public Event GetEvent(int id)
        {
            Event anEvent = null;

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "SELECT t.Id as Id, HomeTeam, Name AS AwayTeam, Status FROM " +
                                         "(SELECT Events.Id AS Id, Name AS HomeTeam, AwayTeam, Status FROM Teams JOIN Events ON Teams.Id = HomeTeam) AS t " +
                                         "JOIN Teams ON Teams.Id = AwayTeam " +
                                         "WHERE t.Id = @Id";

                    using (IDataReader dataReader = dbConnection.ExecuteReader(queryString, new { id }))
                    {
                        
                        while (dataReader.Read())
                        {
                            anEvent = new Event()
                            {
                                Id = Int32.Parse(dataReader["Id"].ToString()),
                                Home = new Team() { Name = dataReader["HomeTeam"].ToString() },
                                Away = new Team() { Name = dataReader["AwayTeam"].ToString() },
                                Status = (Status)Int32.Parse(dataReader["Status"].ToString())
                            };
                        }

                    }

                }

            }
            catch (SQLiteException)
            {
                return null;
            }
            
            return anEvent;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            try
            {
                using (IDbConnection dbConnection = new SQLiteConnection(_connectionString))
                {
                    string queryString = "SELECT t.Id AS Id, HomeTeam, Name as AwayTeam, Status FROM " +
                                         "(SELECT Events.Id AS Id, Name AS HomeTeam, AwayTeam, Status FROM Teams JOIN Events ON Teams.Id = HomeTeam) AS t " +
                                         "JOIN Teams ON Teams.Id = AwayTeam";

                    using (IDataReader dataReader = dbConnection.ExecuteReader(queryString))
                    {
                        while (dataReader.Read())
                        {
                            Event ev = new Event()
                            {
                                Id = Int32.Parse(dataReader["Id"].ToString()),
                                Home = new Team() { Name = dataReader["HomeTeam"].ToString() },
                                Away = new Team() { Name = dataReader["AwayTeam"].ToString() },
                                Status = (Status)Int32.Parse(dataReader["Status"].ToString())
                            };
                            events.Add(ev);
                        }
                    }

                }
            }
            catch (SQLiteException)
            {
                return events;
            }

            return events;
        }

    }
}
