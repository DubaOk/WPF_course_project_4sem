using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Coach_search.Models;
using System.Windows.Controls;
using Coach_search.MVVM.Models;

namespace Coach_search.Data
{
    public class DatabaseHelper
    {
        public string ConnectionString { get; } = "Server=Duba;Database=CoachSearchDb;Trusted_Connection=True;Encrypt=False;Connect Timeout=5;";

        public int AddUser(string name, string email, string passwordHash, Coach_search.Models.UserType userType)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Users (Name, Email, Password, UserType) OUTPUT INSERTED.Id VALUES (@Name, @Email, @Password, @UserType)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", passwordHash);
                    command.Parameters.AddWithValue("@UserType", userType.ToString());
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void AddTutor(Tutor tutor)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Tutors (UserId, Name, Subject, Description, Rating, PricePerHour) VALUES (@UserId, @Name, @Subject, @Description, @Rating, @PricePerHour)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", tutor.UserId);
                    command.Parameters.AddWithValue("@Name", tutor.Name);
                    command.Parameters.AddWithValue("@Subject", (object)tutor.Subject ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)tutor.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Rating", tutor.Rating);
                    command.Parameters.AddWithValue("@PricePerHour", tutor.PricePerHour);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddClient(Client client)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Clients (UserId, Name) VALUES (@UserId, @Name)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", client.UserId);
                    command.Parameters.AddWithValue("@Name", client.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Tutor> GetTutors()
        {
            var tutors = new List<Tutor>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, UserId, Name, Subject, Description, AvatarPath, Rating, PricePerHour FROM Tutors";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tutors.Add(new Tutor
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Subject = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                AvatarPath = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Rating = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                                PricePerHour = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7)
                            });
                        }
                    }
                }
            }
            return tutors;
        }

        public Tutor GetTutorByUserId(int userId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, UserId, Name, Subject, Description, AvatarPath, Rating, PricePerHour FROM Tutors WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Tutor
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Subject = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                AvatarPath = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Rating = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                                PricePerHour = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateTutor(Tutor tutor)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Tutors 
                                SET Name = @Name, Subject = @Subject, Description = @Description, 
                                    AvatarPath = @AvatarPath, Rating = @Rating, PricePerHour = @PricePerHour 
                                WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", tutor.Id);
                    command.Parameters.AddWithValue("@Name", (object)tutor.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Subject", (object)tutor.Subject ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)tutor.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AvatarPath", (object)tutor.AvatarPath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Rating", tutor.Rating);
                    command.Parameters.AddWithValue("@PricePerHour", tutor.PricePerHour);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Booking> GetClientBookings(int clientId)
        {
            var bookings = new List<Booking>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT b.Id, b.TutorId, t.Name, b.DateTime, b.Status 
                        FROM Bookings b
                        JOIN Tutors t ON b.TutorId = t.Id
                        WHERE b.ClientId = @ClientId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookings.Add(new Booking
                            {
                                Id = reader.GetInt32(0),
                                TutorId = reader.GetInt32(1),
                                TutorName = reader.GetString(2),
                                DateTime = reader.GetDateTime(3),
                                Status = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return bookings;
        }

        public void AddReview(Review review)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"INSERT INTO Reviews (ClientId, TutorId, Text, Rating, CreatedAt, BookingId)
                              VALUES (@ClientId, @TutorId, @Text, @Rating, @CreatedAt, @BookingId)";
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", review.ClientId);
                    command.Parameters.AddWithValue("@TutorId", review.TutorId);
                    command.Parameters.AddWithValue("@Text", review.Text);
                    command.Parameters.AddWithValue("@Rating", review.Rating);
                    command.Parameters.AddWithValue("@CreatedAt", review.CreatedAt);
                    command.Parameters.AddWithValue("@BookingId", review.BookingId);
                    command.ExecuteNonQuery();
                }

                string updateRatingQuery = @"UPDATE Tutors 
                                    SET Rating = (SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews WHERE TutorId = @TutorId)
                                    WHERE Id = @TutorId";
                using (var command = new SqlCommand(updateRatingQuery, connection))
                {
                    command.Parameters.AddWithValue("@TutorId", review.TutorId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Review> GetClientReviews(int clientId)
        {
            var reviews = new List<Review>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT r.Id, r.TutorId, r.ClientId, r.Text, r.Rating, r.CreatedAt, r.BookingId, t.Name AS TutorName
                        FROM Reviews r
                        JOIN Tutors t ON r.TutorId = t.Id
                        WHERE r.ClientId = @ClientId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new Review
                            {
                                Id = reader.GetInt32(0),
                                TutorId = reader.GetInt32(1),
                                ClientId = reader.GetInt32(2),
                                Text = reader.GetString(3),
                                Rating = reader.GetInt32(4),
                                CreatedAt = reader.GetDateTime(5),
                                BookingId = reader.GetInt32(6),
                                TutorName = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return reviews;
        }

        public bool HasReviewForBooking(int bookingId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Reviews WHERE BookingId = @BookingId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookingId", bookingId);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public void UpdateBookingStatus(int bookingId, string status)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Bookings SET Status = @Status WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", bookingId);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateClient(Client client)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Clients SET Name = @Name WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", client.UserId);
                    command.Parameters.AddWithValue("@Name", client.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Client GetClient(int userId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT c.UserId, c.Name, u.Email 
                       FROM Clients c
                       JOIN Users u ON c.UserId = u.Id
                       WHERE c.UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                UserId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddBooking(int clientId, int tutorId, DateTime dateTime, string status = "Ожидает")
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string checkQuery = @"SELECT COUNT(*) 
                            FROM Bookings 
                            WHERE TutorId = @TutorId 
                            AND DateTime = @DateTime";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@TutorId", tutorId);
                    checkCommand.Parameters.AddWithValue("@DateTime", dateTime);
                    int existingBookings = (int)checkCommand.ExecuteScalar();

                    if (existingBookings > 0)
                    {
                        throw new Exception("На это время уже записан другой клиент.");
                    }
                }

                string insertQuery = @"INSERT INTO Bookings (ClientId, TutorId, DateTime, Status)
                              VALUES (@ClientId, @TutorId, @DateTime, @Status)";
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@TutorId", tutorId);
                    command.Parameters.AddWithValue("@DateTime", dateTime);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Booking> GetBookingsForTutor(int tutorId, int month, int year)
        {
            var bookings = new List<Booking>();
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine($"Executing GetBookingsForTutor: TutorId={tutorId}, Month={month}, Year={year}");
                    string query = @"SELECT b.Id, b.ClientId, b.TutorId, b.DateTime, b.Status, c.Name AS ClientName
                           FROM Bookings b
                           JOIN Clients c ON b.ClientId = c.UserId
                           WHERE b.TutorId = @TutorId AND MONTH(b.DateTime) = @Month AND YEAR(b.DateTime) = @Year";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TutorId", tutorId);
                        command.Parameters.AddWithValue("@Month", month);
                        command.Parameters.AddWithValue("@Year", year);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var booking = new Booking
                                {
                                    Id = reader.GetInt32(0),
                                    ClientId = reader.GetInt32(1),
                                    TutorId = reader.GetInt32(2),
                                    DateTime = reader.GetDateTime(3),
                                    Status = reader.GetString(4),
                                    ClientName = reader.GetString(5)
                                };
                                bookings.Add(booking);
                                System.Diagnostics.Debug.WriteLine($"Found booking: ID={booking.Id}, Date={booking.DateTime}, ClientName={booking.ClientName}");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error in GetBookingsForTutor: {ex.Message}");
                throw;
            }
            System.Diagnostics.Debug.WriteLine($"GetBookingsForTutor returned {bookings.Count} bookings");
            return bookings;
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection successful.");
                    return true;
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        public List<Review> GetTutorReviews(int tutorId)
        {
            var reviews = new List<Review>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT Id, ClientId, TutorId, Text, Rating, CreatedAt 
                        FROM Reviews 
                        WHERE TutorId = @TutorId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TutorId", tutorId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new Review
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                TutorId = reader.GetInt32(2),
                                Text = reader.GetString(3),
                                Rating = reader.GetInt32(4),
                                CreatedAt = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return reviews;
        }

        public List<Coach_search.Models.User> GetAllUsers()
        {
            var users = new List<Coach_search.Models.User>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, Email, UserType, IsBlocked FROM Users";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Coach_search.Models.User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                UserType = Enum.Parse<Coach_search.Models.UserType>(reader.GetString(3)),
                                IsBlocked = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }
            return users;
        }

        public void ToggleUserBlock(int userId, bool isBlocked)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET IsBlocked = @IsBlocked WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    command.Parameters.AddWithValue("@IsBlocked", isBlocked);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(Coach_search.Models.User user)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET Name = @Name, Email = @Email, UserType = @UserType WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@UserType", user.UserType.ToString());
                    command.ExecuteNonQuery();
                }

                if (user.UserType == Coach_search.Models.UserType.Tutor)
                {
                    string checkTutorQuery = "SELECT COUNT(*) FROM Tutors WHERE UserId = @UserId";
                    using (var command = new SqlCommand(checkTutorQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        int count = (int)command.ExecuteScalar();
                        if (count == 0)
                        {
                            var tutor = new Tutor
                            {
                                UserId = user.Id,
                                Name = user.Name,
                                Rating = 0,
                                PricePerHour = 0
                            };
                            AddTutor(tutor);
                        }
                    }
                }
                else if (user.UserType == Coach_search.Models.UserType.Client)
                {
                    string checkClientQuery = "SELECT COUNT(*) FROM Clients WHERE UserId = @UserId";
                    using (var command = new SqlCommand(checkClientQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        int count = (int)command.ExecuteScalar();
                        if (count == 0)
                        {
                            var client = new Client
                            {
                                UserId = user.Id,
                                Name = user.Name
                            };
                            AddClient(client);
                        }
                    }
                }
            }
        }

        public List<Review> GetAllReviews()
        {
            var reviews = new List<Review>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT r.Id, r.ClientId, r.TutorId, r.Text, r.Rating, r.CreatedAt, r.BookingId, t.Name AS TutorName
                               FROM Reviews r
                               JOIN Tutors t ON r.TutorId = t.Id";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new Review
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                TutorId = reader.GetInt32(2),
                                Text = reader.GetString(3),
                                Rating = reader.GetInt32(4),
                                CreatedAt = reader.GetDateTime(5),
                                BookingId = reader.GetInt32(6),
                                TutorName = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return reviews;
        }

        public void DeleteReview(int reviewId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string getTutorQuery = "SELECT TutorId FROM Reviews WHERE Id = @ReviewId";
                int tutorId;
                using (var command = new SqlCommand(getTutorQuery, connection))
                {
                    command.Parameters.AddWithValue("@ReviewId", reviewId);
                    tutorId = (int)command.ExecuteScalar();
                }

                string deleteQuery = "DELETE FROM Reviews WHERE Id = @ReviewId";
                using (var command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ReviewId", reviewId);
                    command.ExecuteNonQuery();
                }

                string updateRatingQuery = @"UPDATE Tutors 
                                           SET Rating = (SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews WHERE TutorId = @TutorId)
                                           WHERE Id = @TutorId";
                using (var command = new SqlCommand(updateRatingQuery, connection))
                {
                    command.Parameters.AddWithValue("@TutorId", tutorId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}