using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public int TotalContributors { get; set; }
        public decimal? TotalContributions { get; set; }
    }

    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cell { get; set; }
        public DateTime Date { get; set; }
        public bool AlwaysInclude { get; set; }

        public decimal Balance { get; set; }
    }

    //public class ContributorsInViewModel
    //{
    //    public Contributor Contributor { get; set; }
    //    public decimal Balance { get; set; }
    //}

    public class Contribution
    {
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
    }

    public class SimchaContributor
    {
        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal? Amount { get; set; }
        public decimal Balance { get; set; }
        public bool Contributed { get; set; }
    }


    public class Deposit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }

    public class ContributorHistory
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class Manager
    {
        private string _conn;
        public Manager(string conn)
        {
            _conn = conn;
        }

        public IEnumerable<Simcha> GetSimchas()
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas";
            conn.Open();
            List<Simcha> simchas = new List<Simcha>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                simchas.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    TotalContributions = GetSimchaTotal((int)reader["Id"]),
                    TotalContributors = GetSimchaTotalContributors((int)reader["Id"])
                });
            }
            return simchas;
        }

        public void AddSimcha(Simcha s)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Simchas VALUES (@name, @date)";
            cmd.Parameters.AddWithValue("@name", s.Name);
            cmd.Parameters.AddWithValue("@date", s.Date);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public IEnumerable<Contributor> GetContributors()
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors";
            conn.Open();
            List<Contributor> contributors = new List<Contributor>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = GetContributorBalance((int)reader["Id"])
                });
            }
            return contributors;
        }

        public void AddContributor(Contributor c)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributors VALUES (@fName, @lName, @cell, @date, @alwaysInclude) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@fName", c.FirstName);
            cmd.Parameters.AddWithValue("@lName", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.Cell);
            cmd.Parameters.AddWithValue("@date", c.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            conn.Open();
            c.Id = (int)(decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
        }

        public void SubmitDeposit(Deposit d)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposits VALUES (@date, @amount, @contributorId)";
            cmd.Parameters.AddWithValue("@date", d.Date);
            cmd.Parameters.AddWithValue("@amount", d.Amount);
            cmd.Parameters.AddWithValue("@contributorId", d.ContributorId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        //public List<Deposit> GetDepositsForContributor(int id)
        //{
        //    SqlConnection conn = new SqlConnection(_conn);
        //    SqlCommand cmd = conn.CreateCommand();
        //    cmd.CommandText = "SELECT * FROM Deposits WHERE @id=ContributorId";
        //    cmd.Parameters.AddWithValue("@id", id);
        //    conn.Open();
        //    List<Deposit> deposits = new List<Deposit>();
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        deposits.Add(new Deposit
        //        {
        //            Id = (int)reader["Id"],
        //            Date = (DateTime)reader["Date"],
        //            Amount = (decimal)reader["Amount"],
        //            ContributorId = (int)reader["ContributorId"]
        //        });
        //    }
        //    return deposits;
        //}

        //public decimal GetDepositBalance(List<Deposit> deposits)
        //{
        //    decimal counter = 0;
        //    foreach (Deposit d in deposits)
        //    {
        //        counter += d.Amount;
        //    }
        //    return counter;
        //}

        public decimal GetContributorDepositTotal(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Deposits WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public decimal GetContributionTotal(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Contributions WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public decimal GetContributorBalance(int id)
        {
            return GetContributorDepositTotal(id) - GetContributionTotal(id);
        }

        public IEnumerable<ContributorHistory> GetDepositHistory(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Deposits WHERE ContributorId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            List<ContributorHistory> deposits = new List<ContributorHistory>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                deposits.Add(new ContributorHistory
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Action = "Deposit"
                });
            }
            conn.Close();
            conn.Dispose();
            return deposits;
        }

        public Contributor GetContributor(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                };
            }
            return null;
        }

        public void EditContributor(Contributor c)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Contributors SET FirstName=@firstName, LastName=@lastName, Cell=@cell, DateCreated=@dateCreated, AlwaysInclude=@alwaysInclude WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.Cell);
            cmd.Parameters.AddWithValue("@dateCreated", c.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public decimal GetTotalDeposits()
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Deposits";
            conn.Open();
            decimal total = (decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public decimal GetTotalContributions()
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Contributions";
            conn.Open();
            decimal total = (decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public decimal GetTotalBalance()
        {
            return GetTotalDeposits() - GetTotalContributions();
        }

        public List<SimchaContributor> GetContributorsThatContributed(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions c JOIN Contributors co ON co.id=c.ContributorId WHERE SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            List<SimchaContributor> contributors = new List<SimchaContributor>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributors.Add(new SimchaContributor
                {
                    ContributorId = (int)reader["ContributorId"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Amount = (decimal)reader["Amount"],
                    Contributed = true
                });
            }
            conn.Close();
            conn.Dispose();
            return contributors;
        }

        public Simcha GetSimchaById(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                };
            }
            conn.Close();
            conn.Dispose();
            return null;
        }

        public void UpdateContribution(IEnumerable<SimchaContributor> contributions, int simchaId)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributions VALUES (@contributorId,@simchaId,@amount)";
            conn.Open();
            foreach (SimchaContributor sc in contributions)
            {
                if (sc.Contributed)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@contributorId", sc.ContributorId);
                    if (sc.Amount == null)
                    {
                        cmd.Parameters.AddWithValue("@amount", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@amount", sc.Amount);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            conn.Close();
            conn.Dispose();
        }

        public void DeleteContributions(int simchaId)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", simchaId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public decimal GetSimchaTotal(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Contributions WHERE SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            decimal total;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                total = (decimal)cmd.ExecuteScalar();
            }
            else
            {
                total = 0;
            }
            conn.Close();
            conn.Dispose();
            return total;
        }

        public int GetSimchaTotalContributors(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributions WHERE simchaId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int total = (int)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public int GetContributorCount()
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return count;
        }

        public List<ContributorHistory> GetContributionsHistory(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions c JOIN Simchas s ON s.Id=c.SimchaId WHERE c.ContributorId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            List<ContributorHistory> contributions = new List<ContributorHistory>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributions.Add(new ContributorHistory
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["ContributedDate"],
                    Action = $"Contribution for {(string)reader["Name"]} Simcha"
                });
            }
            conn.Close();
            conn.Dispose();
            return contributions;
        }

        public decimal? GetAmount(int id)
        {
            SqlConnection conn = new SqlConnection(_conn);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Amount FROM Contributions c JOIN Simchas s ON s.Id=c.SimchaId WHERE c.SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            decimal? amount = (decimal?)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return amount;
        }

        //    public List<string> GetContributorNamesBySimcha(int id)
        //    {
        //        SqlConnection conn = new SqlConnection(_conn);
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "SELECT * FROM Contributors c JOIN SimchaContributors sc ON c.Id=sc.ContributorId WHERE sc.SimchaId=@id";
        //        cmd.Parameters.AddWithValue("@id", id);
        //        conn.Open();
        //        List<string> contributors = new List<string>();
        //        string name = "";
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            name += (string)reader["FirstName"];
        //            name += (string)reader["LastName"];
        //            contributors.Add(name);
        //        }

        //        return contributors;
        //    }



        //    //public List<Contribution> GetContributionForSimcha(int id)
        //    //{
        //    //    SqlConnection conn = new SqlConnection(_conn);
        //    //    SqlCommand cmd = conn.CreateCommand();
        //    //    cmd.CommandText = "SELECT * FROM SimchaContributors sc JOIN Contributors c ON c.Id=sc.ContributorId WHERE sc.SimchaId=@id";
        //    //    cmd.Parameters.AddWithValue("@id", id);
        //    //    conn.Open();
        //    //    List<Contribution> contributions = new List<Contribution>();
        //    //    SqlDataReader reader = cmd.ExecuteReader();
        //    //    while (reader.Read())
        //    //    {
        //    //        contributions.Add(new Contribution
        //    //        {
        //    //            SimchaId = (int)reader["SimchaId"],
        //    //            ContributorId = (int)reader["ContributorId"],
        //    //            Amount = (decimal)reader["Amount"]
        //    //        });
        //    //    }
        //    //    conn.Close();
        //    //    conn.Dispose();
        //    //    return contributions;
        //    //}

        //    public List<Contributor> GetContributorsBySimcha(int id)
        //    {
        //        SqlConnection conn = new SqlConnection(_conn);
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "SELECT * FROM Contributors c JOIN SimchaContributors sc ON c.Id=sc.ContributorId WHERE sc.SimchaId=@id";
        //        cmd.Parameters.AddWithValue("@id", id);
        //        conn.Open();
        //        List<Contributor> contributors = new List<Contributor>();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            contributors.Add(new Contributor
        //            {
        //                Id = (int)reader["Id"],
        //                FirstName = (string)reader["FirstName"],
        //                LastName = (string)reader["LastName"],
        //                Cell = (string)reader["Cell"],
        //                Date = (DateTime)reader["Date"],
        //                AlwaysInclude = (bool)reader["AlwaysInclude"]
        //            });
        //        }
        //        return contributors;
        //    }

        //    public string GetSimchaNameById(int id)
        //    {
        //        SqlConnection conn = new SqlConnection(_conn);
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "SELECT Name FROM Simchas WHERE Id=@id";
        //        cmd.Parameters.AddWithValue("@id", id);
        //        conn.Open();
        //        string name = (string)cmd.ExecuteScalar();
        //        conn.Close();
        //        conn.Dispose();
        //        return name;
        //    }

        //    //public decimal GetTotal(List<decimal> balances)
        //    //{
        //    //    decimal counter = 0;
        //    //    foreach (decimal d in balances)
        //    //    {
        //    //        counter += d;
        //    //    }
        //    //    return counter;
        //    //}


        //    public void EditContributor(Contributor c)
        //    {
        //        SqlConnection conn = new SqlConnection(_conn);
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandText = "UPDATE Contributors SET FirstName=@firstName, LastName=@lastName, Cell=@cell, DateCreated=@dateCreated, AlwaysInclude=@alwaysInclude WHERE id=@id";
        //        cmd.Parameters.AddWithValue("@id", c.Id);
        //        cmd.Parameters.AddWithValue("@firstName", c.FirstName);
        //        cmd.Parameters.AddWithValue("@lastName", c.LastName);
        //        cmd.Parameters.AddWithValue("@cell", c.Cell);
        //        cmd.Parameters.AddWithValue("@dateCreated", c.Date);
        //        cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        conn.Dispose();
        //    }
    }
}
