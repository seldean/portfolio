///
/// This document is wrote by Sertan Balta, please contact with him directly if it needs any fixes, updates or optimization changes.
///

using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace Seldean_CredentialManager
{
    public static class ServerCredentials
    {
        #region PROPERTIES
        public static string SERVER_ADDRESS { get; private set; } = string.Empty;
        public static string DATABASE_NAME { get; private set; } = string.Empty;
        public static string DATABASE_USER_NAME { get; private set; } = string.Empty;
        public static string DATABASE_USER_PASSWORD { get; private set; } = string.Empty;
        #endregion

        #region VARIABLES
        private static string CONNECTION_STRING = string.Empty;
        #endregion

        /// <summary>
        /// CredentialType is a list of available properties.
        /// </summary>
        public enum CredentialType
        {
            // Any new property should be added here.
            // boolean Encrypt and boolean TrustServerCertificate may be good examples for this.
            ServerAddress,
            DatabaseName,
            DatabaseUserName,
            DatabasePassword,
        }

        #region METHODS
        /// <summary>
        /// This method is already being called by ChangeCredentialData method.
        /// You may consider using this method for manual refresh when necessary.
        /// </summary>
        /// <returns>True if connection is successful. False if there is any kind of an error.</returns>
        public static bool RefreshConnection()
        {
            var connection_builder = new SqlConnectionStringBuilder
            {
                DataSource = SERVER_ADDRESS,
                InitialCatalog = DATABASE_NAME,
                UserID = DATABASE_USER_NAME,
                Password = DATABASE_USER_PASSWORD,
                Encrypt = true,
                TrustServerCertificate = false
            };
            CONNECTION_STRING = connection_builder.ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
                {
                    try
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand($"PRINT 'INFO: Connection opened successfully by: {LANProcesses.GetLocalIPAddress()}'", con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"{e.Message}", "ERROR IN REFRESH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Couldn't reach to database.\n{e.Message}", "ERROR IN REFRESH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Use it for changing the data of ServerCredentials secured properties.
        /// </summary>
        /// <param name="ct">To specify which credential is being changed.</param>
        /// <param name="newData">New data for the property.</param>
        /// <returns>True if change was successful.</returns>
        public static bool ChangeCredentialData(CredentialType ct, string? newData)
        {
            if (string.IsNullOrWhiteSpace(newData))
            {
                Debug.Print($"Attempt to set empty credential for {ct}");
                return false;
            }

            string currentValue = ct switch
            {
                CredentialType.ServerAddress => SERVER_ADDRESS,
                CredentialType.DatabaseName => DATABASE_NAME,
                CredentialType.DatabaseUserName => DATABASE_USER_NAME,
                CredentialType.DatabasePassword => DATABASE_USER_PASSWORD,
                _ => throw new ArgumentOutOfRangeException(nameof(ct), ct, null)
            };

            if (currentValue == newData)
            {
                Debug.Print($"New value for {ct} is same as current value");
                return false;
            }

            try
            {
                return ct switch
                {
                    CredentialType.ServerAddress => (SERVER_ADDRESS = newData) == newData,
                    CredentialType.DatabaseName => (DATABASE_NAME = newData) == newData,
                    CredentialType.DatabaseUserName => (DATABASE_USER_NAME = newData) == newData,
                    CredentialType.DatabasePassword => (DATABASE_USER_PASSWORD = newData) == newData,
                    _ => false
                };
            }
            catch (Exception e)
            {
                Debug.Print($"Failed to update {ct}: {e.Message}");
                return false;
            }
            finally
            {
                RefreshConnection();
            }
        }
        /// <summary>
        /// This alternative is used to insert data with a collection.
        /// </summary>
        /// <param name="newDataCollection">Needs to be in the same order with CredentialType items.</param>
        /// <returns>True if changes was successful.</returns>
        public static bool ChangeCredentialData(string[]? newDataCollection)
        {
            string[] credentialTypes = Enum.GetNames(typeof(CredentialType));

            if (newDataCollection == null || newDataCollection.Length != credentialTypes.Length)
            {
                Debug.Print("Invalid input: Array is null or length doesn't match with CredentialType.Length");
                return false;
            }

            for (int i = 0; i < credentialTypes.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(newDataCollection[i]))
                {
                    Debug.Print($"Attempt to set empty credential for {credentialTypes[i]}");
                    return false;
                }
            }

            try
            {
                for (int i = 0; i < credentialTypes.Length; i++)
                {
                    CredentialType currentType = (CredentialType)Enum.Parse(typeof(CredentialType), credentialTypes[i]);
                    string currentValue = currentType switch
                    {
                        CredentialType.ServerAddress => SERVER_ADDRESS,
                        CredentialType.DatabaseName => DATABASE_NAME,
                        CredentialType.DatabaseUserName => DATABASE_USER_NAME,
                        CredentialType.DatabasePassword => DATABASE_USER_PASSWORD,
                        _ => throw new Exception("Argument could be out of reach.")
                    };

                    if (currentValue == newDataCollection[i])
                        continue;

                    switch (currentType)
                    {
                        case CredentialType.ServerAddress:
                            SERVER_ADDRESS = newDataCollection[i];
                            break;
                        case CredentialType.DatabaseName:
                            DATABASE_NAME = newDataCollection[i];
                            break;
                        case CredentialType.DatabaseUserName:
                            DATABASE_USER_NAME = newDataCollection[i];
                            break;
                        case CredentialType.DatabasePassword:
                            DATABASE_USER_PASSWORD = newDataCollection[i];
                            break;
                    }
                }

                return RefreshConnection();
            }
            catch (Exception e)
            {
                Debug.Print($"Failed to update credentials: {e.Message}");
                return false;
            }
        }
        #endregion
    }
}
