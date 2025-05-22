using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Coach_search.Models;
using System.Windows.Controls;
using Coach_search.MVVM.Models;
using Coach_search.ViewModels;

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

        public bool EmailExists(string email)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void AddTutor(Tutor tutor)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Tutors (UserId, Name, Subject, Description, AvatarPath, Rating, PricePerHour, IsVisible) VALUES (@UserId, @Name, @Subject, @Description, @AvatarPath, @Rating, @PricePerHour, @IsVisible)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", tutor.UserId);
                    command.Parameters.AddWithValue("@Name", tutor.Name);
                    command.Parameters.AddWithValue("@Subject", (object)tutor.Subject ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)tutor.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AvatarPath", (object)tutor.AvatarPath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Rating", tutor.Rating);
                    command.Parameters.AddWithValue("@PricePerHour", tutor.PricePerHour);
                    command.Parameters.AddWithValue("@IsVisible", tutor.IsVisible);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddClient(Client client)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Clients (UserId, Name, AvatarPath) VALUES (@UserId, @Name, @AvatarPath)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", client.UserId);
                    command.Parameters.AddWithValue("@Name", client.Name);
                    command.Parameters.AddWithValue("@AvatarPath", (object)client.AvatarPath ?? DBNull.Value);
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
                string query = @"
            SELECT t.Id, t.UserId, t.Name, t.Subject, t.Description, t.AvatarPath, t.Rating, t.PricePerHour, t.IsVisible 
            FROM Tutors t
            JOIN Users u ON t.UserId = u.Id
            WHERE t.IsVisible = 1 AND u.IsBlocked = 0"; // Добавляем проверку на IsBlocked
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
                                PricePerHour = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                                IsVisible = reader.GetBoolean(8)
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
                string query = "SELECT Id, UserId, Name, Subject, Description, AvatarPath, Rating, PricePerHour, IsVisible FROM Tutors WHERE UserId = @UserId";
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
                                PricePerHour = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                                IsVisible = reader.GetBoolean(8)
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
                string tutorQuery = @"UPDATE Tutors 
                                     SET Name = @Name, Subject = @Subject, Description = @Description, 
                                         AvatarPath = @AvatarPath, Rating = @Rating, PricePerHour = @PricePerHour, 
                                         IsVisible = @IsVisible 
                                     WHERE Id = @Id";
                using (var command = new SqlCommand(tutorQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", tutor.Id);
                    command.Parameters.AddWithValue("@Name", (object)tutor.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Subject", (object)tutor.Subject ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)tutor.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AvatarPath", (object)tutor.AvatarPath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Rating", tutor.Rating);
                    command.Parameters.AddWithValue("@PricePerHour", tutor.PricePerHour);
                    command.Parameters.AddWithValue("@IsVisible", tutor.IsVisible);
                    command.ExecuteNonQuery();
                }

                string userQuery = "UPDATE Users SET Name = @Name WHERE Id = @UserId";
                using (var command = new SqlCommand(userQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", tutor.UserId);
                    command.Parameters.AddWithValue("@Name", (object)tutor.Name ?? DBNull.Value);
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
                string query = @"
            SELECT b.Id, b.TutorId, COALESCE(t.Name, 'Репетитор не найден') AS TutorName, 
                   b.DateTime, b.Status, u.IsBlocked
            FROM Bookings b
            LEFT JOIN Tutors t ON b.TutorId = t.Id
            LEFT JOIN Users u ON t.UserId = u.Id
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
                                Status = reader.GetString(4),
                                IsTutorBlocked = reader.IsDBNull(5) ? false : reader.GetBoolean(5) // Добавляем информацию о блокировке
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

        public void DeleteBooking(int bookingId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM Bookings WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", bookingId);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("Запись не найдена или уже удалена.");
                    }
                }
            }
        }

        public void UpdateClient(Client client)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string clientQuery = "UPDATE Clients SET Name = @Name, AvatarPath = @AvatarPath WHERE UserId = @UserId";
                using (var command = new SqlCommand(clientQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", client.UserId);
                    command.Parameters.AddWithValue("@Name", client.Name);
                    command.Parameters.AddWithValue("@AvatarPath", (object)client.AvatarPath ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }

                string userQuery = "UPDATE Users SET Name = @Name WHERE Id = @UserId";
                using (var command = new SqlCommand(userQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", client.UserId);
                    command.Parameters.AddWithValue("@Name", client.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateClientAvatar(int userId, string avatarPath)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Clients SET AvatarPath = @AvatarPath WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@AvatarPath", (object)avatarPath ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Client GetClient(int userId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT c.UserId, c.Name, u.Email, c.AvatarPath 
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
                                Email = reader.GetString(2),
                                AvatarPath = reader.IsDBNull(3) ? null : reader.GetString(3)
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
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
                    SELECT b.Id, b.ClientId, COALESCE(c.Name, 'Клиент не найден') AS ClientName, 
                           b.DateTime, b.Status, 
                           CAST(CASE WHEN u.IsBlocked IS NULL THEN 0 ELSE u.IsBlocked END AS BIT) as IsClientBlocked
                    FROM Bookings b
                    LEFT JOIN Clients c ON b.ClientId = c.UserId
                    LEFT JOIN Users u ON c.UserId = u.Id
                    WHERE b.TutorId = @TutorId 
                    AND MONTH(b.DateTime) = @Month 
                    AND YEAR(b.DateTime) = @Year";

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
                                ClientName = reader.GetString(2),
                                DateTime = reader.GetDateTime(3),
                                Status = reader.GetString(4),
                                IsClientBlocked = reader.GetBoolean(5)
                            };
                            System.Diagnostics.Debug.WriteLine($"Booking loaded: ID={booking.Id}, ClientName={booking.ClientName}, IsBlocked={booking.IsClientBlocked}");
                            bookings.Add(booking);
                        }
                    }
                }
            }
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
                string query = @"
                    SELECT u.Id, u.Name, u.Email, u.UserType, u.IsBlocked,
                        CASE 
                            WHEN u.UserType = 'Tutor' THEN (SELECT AvatarPath FROM Tutors WHERE UserId = u.Id)
                            ELSE (SELECT AvatarPath FROM Clients WHERE UserId = u.Id)
                        END AS AvatarPath
                    FROM Users u";
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
                                IsBlocked = reader.GetBoolean(4),
                                AvatarPath = reader.IsDBNull(5) ? null : reader.GetString(5)
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
                                PricePerHour = 0,
                                IsVisible = false
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
                                Name = user.Name,
                                AvatarPath = null
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

        public void UpdateReview(Review review)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string updateQuery = @"UPDATE Reviews 
                                      SET Text = @Text, Rating = @Rating 
                                      WHERE Id = @Id";
                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", review.Id);
                    command.Parameters.AddWithValue("@Text", review.Text);
                    command.Parameters.AddWithValue("@Rating", review.Rating);
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

        public void AddMessage(int senderId, int receiverId, string content, DateTime sentAt)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Messages (SenderId, ReceiverId, Content, SentAt) VALUES (@SenderId, @ReceiverId, @Content, @SentAt)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SenderId", senderId);
                    command.Parameters.AddWithValue("@ReceiverId", receiverId);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@SentAt", sentAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Message> GetMessagesBetween(int userId1, int userId2)
        {
            var messages = new List<Message>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
                    SELECT Id, SenderId, ReceiverId, Content, SentAt 
                    FROM Messages 
                    WHERE (SenderId = @UserId1 AND ReceiverId = @UserId2) 
                       OR (SenderId = @UserId2 AND ReceiverId = @UserId1) 
                    ORDER BY SentAt ASC";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId1", userId1);
                    command.Parameters.AddWithValue("@UserId2", userId2);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new Message
                            {
                                Id = reader.GetInt32(0),
                                SenderId = reader.GetInt32(1),
                                ReceiverId = reader.GetInt32(2),
                                Content = reader.GetString(3),
                                SentAt = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return messages;
        }

        public List<int> GetMessageContacts(int userId)
        {
            var contacts = new List<int>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
                    SELECT DISTINCT 
                        CASE 
                            WHEN SenderId = @UserId THEN ReceiverId 
                            ELSE SenderId 
                        END as ContactId
                    FROM Messages
                    WHERE SenderId = @UserId OR ReceiverId = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contacts.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return contacts;
        }

        public User GetUserById(int userId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, Email, Password, UserType, IsBlocked FROM Users WHERE Id = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                UserType = (UserType)Enum.Parse(typeof(UserType), reader.GetString(4)),
                                IsBlocked = reader.GetBoolean(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool HasBookingBetween(int user1Id, int user2Id)
        {
            using var connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                // Сначала проверяем, является ли user1Id репетитором
                var command = new SqlCommand(
                    @"WITH UserBookings AS (
                        -- Проверяем случай, когда user1Id - репетитор
                        SELECT COUNT(*) as cnt
                        FROM Bookings b
                        JOIN Tutors t ON b.TutorId = t.Id
                        WHERE t.UserId = @User1Id 
                        AND b.ClientId = @User2Id
                        AND b.Status != 'Отменено'
                        
                        UNION ALL
                        
                        -- Проверяем случай, когда user2Id - репетитор
                        SELECT COUNT(*)
                        FROM Bookings b
                        JOIN Tutors t ON b.TutorId = t.Id
                        WHERE t.UserId = @User2Id 
                        AND b.ClientId = @User1Id
                        AND b.Status != 'Отменено'
                    )
                    SELECT SUM(cnt) FROM UserBookings",
                    connection);

                command.Parameters.AddWithValue("@User1Id", user1Id);
                command.Parameters.AddWithValue("@User2Id", user2Id);

                var result = command.ExecuteScalar();
                int count = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                
                System.Diagnostics.Debug.WriteLine($"HasBookingBetween: User1Id={user1Id}, User2Id={user2Id}, Count={count}");
                return count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in HasBookingBetween: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }
    }
}