using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SimpleAds.Data
{
    public class SimpleAdManager
    {
        private readonly string _connectionString;

        public SimpleAdManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddAd(Ad ad)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Ads (Title, Description, Name, Date, PhoneNumber)
                                     VALUES (@title, @description, @name, @date, @phone); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@title", ad.Title);
                cmd.Parameters.AddWithValue("@description", ad.Description);
                cmd.Parameters.AddWithValue("@name", ad.Name);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@phone", ad.PhoneNumber);
                connection.Open();
                ad.Id = (int)(decimal)cmd.ExecuteScalar();

                cmd.CommandText = "INSERT INTO Images (FileName, AdId) VALUES (@fileName, @adId)";

                foreach (string image in ad.Images)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@adId", ad.Id);
                    cmd.Parameters.AddWithValue("@fileName", image);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Ad> GetAdsWithSingleImages()
        {
            List<Ad> ads = new List<Ad>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "GetAdsWithSingleImage";
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Ad ad = new Ad
                    {
                        Date = (DateTime)reader["Date"],
                        Description = (string)reader["Description"],
                        Id = (int)reader["Id"],
                        Images = new List<string>
                        {
                            (string) reader["FileName"]
                        },
                        Name = (string)reader["Name"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        Title = (string)reader["Title"]
                    };
                    ads.Add(ad);
                }
            }

            return ads;
        }

        public Ad GetAdById(int adId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT a.*, i.FileName FROM Ads a
                                    JOIN Images i ON a.id = i.AdId
                                    WHERE a.Id = @id";
                cmd.Parameters.AddWithValue("@id", adId);
                connection.Open();
                var reader = cmd.ExecuteReader();

                reader.Read();
                Ad ad = new Ad
                {
                    Date = (DateTime)reader["Date"],
                    Description = (string)reader["Description"],
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Title = (string)reader["Title"]
                };

                List<string> images = new List<string>();
                images.Add((string)reader["FileName"]);
                while (reader.Read())
                {
                    images.Add((string)reader["FileName"]);
                }
                ad.Images = images;
                return ad;
            }


        }

    }
}