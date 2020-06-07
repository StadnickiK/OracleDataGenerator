using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SklepGenerator.OracleDataManager;

namespace SklepGenerator.Connection
{
    public class OracleDB
    {
        public OracleConnection OConnection { get; set; }
        public string ConString { get; set; }

        DataGenerator DataGenerator = new DataGenerator();
        DataLoader DataLoader { get; set; }
        FileManager _fileManager = new FileManager();
        public DataInserter DataInserter { get; set; }

        public OracleDB(string conString) {
            ConString = conString;
        }

        public void Connect()
        {
            OConnection = new OracleConnection
            {
                ConnectionString = ConString
            };
            OConnection.Open();
            this.DataLoader = new DataLoader(OConnection);
            DataInserter = new DataInserter(OConnection);
            Console.WriteLine("Connected to Oracle " + OConnection.ServerVersion);
        }
        public Dictionary<string, Table> GetDataScheme() { return DataLoader.DataScheme; }

        public void Connect(string conString)
        {
            OConnection = new OracleConnection
            {
                ConnectionString = conString
            };
            OConnection.Open();
            Console.WriteLine("Connected to Oracle " + OConnection.ServerVersion);
        }

        public void Close()
        {
            OConnection.Close();
            OConnection.Dispose();
            Console.WriteLine("Connection closed");
        }

        public void LoadData()
        {
            DataLoader.LoadDB();
        }

        public int GetLastID(string tableName)
        {
            return DataLoader.GetLastID(tableName);
        }

        public List<int> GetUnusedIDs(string tableName)
        {
            return DataLoader.GetUnusedRowsIds(tableName);
        }

        public List<int> GetUnusedIDsOfRelations(string tableName)
        {
            return DataLoader.GetUnusedIDsOfRelations(tableName);
        }


        public int InsertRandValues(string tableName)
        {
            Table t = DataLoader.DataScheme[tableName];
            string data = "";
            int i = 0;
            int id = 0;
            foreach(DataTypes.DataType d in t.Columns)
            {
                DateTime date = new DateTime(1970,1,1);
                if (d.Value == "DATE")
                {
                    date = DataLoader.GetLastDate(tableName, d.Name);
                }
                if (t.Relations.ContainsKey(d.Name))
                {
                    if(DataLoader.DataScheme[t.Relations[d.Name]].FreeIDs.Count != 0)
                    {
                        int freeID = DataLoader.DataScheme[t.Relations[d.Name]].FreeIDs[DataGenerator.GetRandNumber(0,
                            DataLoader.DataScheme[t.Relations[d.Name]].FreeIDs.Count)];
                        DataLoader.DataScheme[t.Relations[d.Name]].FreeIDs.Remove(freeID);
                        data += freeID;
                    }
                    else
                    {
                        data += InsertRandValues(t.Relations[d.Name]).ToString();
                    }
                }
                else
                {

                    if (i == t.Columns.Count - 1)
                    {
                        data += DataGenerator.GenerateRandData(d.Value, date);
                    }
                    else if (i == 0)
                    {
                        id = GetLastID(tableName);
                        id++;
                        data += id.ToString();
                    }
                    else
                    {
                        data += DataGenerator.GenerateRandData(d.Value, date);
                    }
                }
                if (i != t.Columns.Count - 1)
                    data += ",";
                i++;
            }
            string s = DataInserter.InsertData(data, tableName);
            Console.WriteLine(s);
            _fileManager.AddLine(s);
            return id;
        }

        public int InsertRandValues(string tableName, int count)
        {
            Console.WriteLine("Inserting " + count + " rows into " + tableName);
            for (int i = 0; i < count;i++)
            {
                InsertRandValues(tableName);
            }
            Console.WriteLine("Inserting finished");
            _fileManager.SaveToFile(tableName);
            return 0;
        }

    }
}
