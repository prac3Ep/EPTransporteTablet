using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EPTransporte.Models;

namespace EPTransporte.Clases
{
    public class Conexion
    {
        private const string V = @"Data Source = 192.168.50.11; Initial Catalog=TransporteEPTest; Persist Security Info=True;User ID=sa;Password=DesarrolloEP1*";

       // public static string sConn = @"Server=localhost\SQLEXPRESS01;Database=TransporteEPTest;User ID=sa;Password=DesarrolloEP1*;TrustServerCertificate=True;";

        private static string sConn = V;//@np4jr#10!

        // private static string sConn = @"Data Source= 192.168.50.168,1433;Network Library=DBMSSOCN;Initial Catalog=myDataBase;User ID=sa;Password=@np4jr#10!;Connect Timeout=10";

        //private static string sConn = @"Server=192.168.50.168;Database=TransporteEP;Trusted_Connection=True;";

        private static SqlCommand comm;
        private static SqlConnection conn;
        private static DataTable dt;
        private static SqlDataAdapter da;

        public static void GuardarEntrada(EntradaModel model)
        {
            SqlConnection conn = null;
            try
            {
                using (comm = new SqlCommand("Ins_Entrada", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@Vacio", model.Vacio);
                    comm.Parameters.AddWithValue("@Cargado", model.Cargado);
                    comm.Parameters.AddWithValue("@Botando", model.Botando);
                    comm.Parameters.AddWithValue("@NumCaja", string.IsNullOrEmpty(model.NumCaja) ? (object)DBNull.Value : model.NumCaja);
                    comm.Parameters.AddWithValue("@NumSello", string.IsNullOrEmpty(model.NumSello) ? (object)DBNull.Value : model.NumSello);
                    comm.Parameters.AddWithValue("@TransportistaNombre", model.TransportistaNombre ?? "");
                    comm.Parameters.AddWithValue("@OperadorNombre", model.OperadorNombre ?? "");
                    comm.Parameters.AddWithValue("@EconomicoNombre", model.EconomicoNombre ?? "");
                    comm.Parameters.AddWithValue("@SitioEP", model.SucursalNombre ?? "");

                    conn.Open();
                    comm.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                // Log específico para errores SQL
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Number} - {sqlEx.Message}");
                throw new Exception("Error de base de datos al guardar la entrada: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex}");
                throw new Exception("Error al guardar la entrada: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static void GuardarEntradaGeneral(EntradaGeneralModel model)
        {
            try
            {
                using (comm = new SqlCommand("Ins_Entrada_General", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@Vacio", model.Vacio);
                    comm.Parameters.AddWithValue("@Cargado", model.Cargado);
                    comm.Parameters.AddWithValue("@Botando", model.Botando);
                    comm.Parameters.AddWithValue("@NumCaja", string.IsNullOrEmpty(model.NumCaja) ? (object)DBNull.Value : model.NumCaja);
                    comm.Parameters.AddWithValue("@NumSello", string.IsNullOrEmpty(model.NumSello) ? (object)DBNull.Value : model.NumSello);
                    comm.Parameters.AddWithValue("@TransportistaNombre", model.TransportistaNombre);
                    comm.Parameters.AddWithValue("@OperadorNombre", model.OperadorNombre);
                    comm.Parameters.AddWithValue("@EconomicoNombre", model.EconomicoNombre);
                    comm.Parameters.AddWithValue("@SitioEP", model.SitioEP);

                    conn.Open();
                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la entrada: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static void ActualizarFechaSalida(int idSalida)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Fecha_Salida", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@IdSalida", idSalida);
                    conn.Open();
                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar fecha de salida: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static List<DatosSalida> ObtenerSalidasPendientesPorSucursal(string sucursal)
        {
            dt = new DataTable();
            List<DatosSalida> oSalidas = new List<DatosSalida>();
            try
            {
                using (comm = new SqlCommand("Sel_Salidas_Reg_Por_Sucursal", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Sucursal", sucursal);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            DatosSalida oSalida = new DatosSalida
                            {
                                IdPaseSalida = int.Parse(dr[0].ToString()),
                                Folio = dr[1].ToString(),
                                Economico = dr[2].ToString(),
                                Transportista = dr[3].ToString()
                            };
                            oSalidas.Add(oSalida);
                        }
                    }
                }
                dt.Dispose();
            }
            catch (Exception ex)
            {
                conn.Close();
                dt.Dispose();
                comm.Dispose();
                throw new Exception("Error al obtener salidas por sucursal: " + ex.Message);
            }
            return oSalidas;
        }

        internal static DataTable ObtenerOperadoresPorTransportistaNombre(string transportistaNombre)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Operadores_Por_Transportista_Nombre", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@TransportistaNombre", transportistaNombre);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operadores por transportista: " + ex.Message);
            }
            return dt;
        }

        internal static DataTable ObtenerEconomicosPorTransportistaNombre(string transportistaNombre)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Economicos_Por_Transportista_Nombre", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@TransportistaNombre", transportistaNombre);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economicos por transportista: " + ex.Message);
            }
            return dt;
        }

       

        internal static DataTable ObtenerOperadoresPorTransportista(int idTransportista)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            SqlCommand comm = null;
            SqlDataAdapter da = null;

            try
            {
                conn = new SqlConnection(sConn);
                comm = new SqlCommand("Sel_Operadores_Por_Transportista", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                da = new SqlDataAdapter(comm);

                conn.Open();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operadores: " + ex.Message);
            }
            finally
            {
                // Cerrar en orden inverso a la apertura
                da?.Dispose();

                comm?.Dispose();

                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }

        internal static DataTable ObtenerEconomicosPorTransportista(int idTransportista)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            SqlCommand comm = null;
            SqlDataAdapter da = null;

            try
            {
                conn = new SqlConnection(sConn);
                comm = new SqlCommand("Sel_Economicos_Por_Transportista", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                da = new SqlDataAdapter(comm);

                conn.Open();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economicos: " + ex.Message);
            }
            finally
            {
                da?.Dispose();

                comm?.Dispose();

                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }


       

        // Obtener operador por ID
        internal static DataRow ObtenerOperadorPorId(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_OperadorPorId", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operador: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static DataRow ObtenerEconomicoPorId(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_EconomicoPorId", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economico: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

       

       

        internal static DataTable ObtenerTransportistas()
        {
            DataTable dt = new DataTable();

            try
            {
                using (comm = new SqlCommand("Sel_Transportistas", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al obtener transportistas: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }

        

        // Obtener transportista por ID
        internal static DataRow ObtenerTransportistaPorId(int id)
        {
            DataTable dt = new DataTable();

            try
            {
                using (comm = new SqlCommand("Sel_TransportistaPorId", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al obtener transportista: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

       

        internal static DataTable ObtenerUsuarios()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Usuarios", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuarios: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        internal static List<Sucursal> ObtenerSucursales()
        {
            var sucursales = new List<Sucursal>();
            try
            {
                using (var comm = new SqlCommand("Sel_Sucursales", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sucursales.Add(new Sucursal
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SitioEP = reader["SitioEP"].ToString(),
                                Ubicacion = reader["Ubicacion"].ToString(),
                                Habilitado = Convert.ToBoolean(reader["Habilitado"])
                            });
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                throw new Exception("Error al obtener sucursales: " + ex.Message);
            }
            return sucursales;
        }

        internal static DataTable ObtenerSucursalesDisponibles()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Sucursales_Disponibles", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
                throw new Exception("Error al obtener sucursales: " + ex.Message);
            }
            return dt;
        }


       

        internal static EditSalida ObtenerSalidaEditar(int id)
        {
            EditSalida oSalidaEditar = new EditSalida();
            dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Salida", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            oSalidaEditar.IdSalida = Int64.Parse(dr[0].ToString());
                            oSalidaEditar.SucursalEP = dr[1].ToString();
                            oSalidaEditar.Transportista = dr[2].ToString();
                            oSalidaEditar.Operador = dr[3].ToString();
                            oSalidaEditar.Economico = dr[4].ToString();
                            oSalidaEditar.LocalViaje = bool.Parse(dr[5].ToString());
                            oSalidaEditar.Cabina = bool.Parse(dr[6].ToString());
                            oSalidaEditar.Cargado = bool.Parse(dr[7].ToString());
                            oSalidaEditar.Vacio = bool.Parse(dr[8].ToString());
                            oSalidaEditar.Expo = bool.Parse(dr[9].ToString());
                            oSalidaEditar.Caja = bool.Parse(dr[10].ToString());
                            oSalidaEditar.NumCaja = dr[11].ToString();
                            oSalidaEditar.NumSello = dr[12].ToString();
                            oSalidaEditar.Auditor = dr[13].ToString();
                            oSalidaEditar.Folio = dr[14].ToString();
                        }
                    }
                    dt.Dispose();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
            }
            return oSalidaEditar;
        }

        internal static void GuardarEditar(EditSalida oSalidaEditar)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Salida_Reg", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@idSalida", oSalidaEditar.IdSalida);
                    comm.Parameters.AddWithValue("@SucursalEP", oSalidaEditar.SucursalEP);
                    comm.Parameters.AddWithValue("@Transportista", oSalidaEditar.Transportista);
                    comm.Parameters.AddWithValue("@Operador", oSalidaEditar.Operador);
                    comm.Parameters.AddWithValue("@Economico", oSalidaEditar.Economico);
                    comm.Parameters.AddWithValue("@LocalViaje", oSalidaEditar.LocalViaje);
                    comm.Parameters.AddWithValue("@Cabina", oSalidaEditar.Cabina);
                    comm.Parameters.AddWithValue("@Cargado", oSalidaEditar.Cargado);
                    comm.Parameters.AddWithValue("@Vacio", oSalidaEditar.Vacio);
                    comm.Parameters.AddWithValue("@Exp", oSalidaEditar.Expo);
                    comm.Parameters.AddWithValue("@Caja", oSalidaEditar.Caja);
                    comm.Parameters.AddWithValue("@NumCaja", oSalidaEditar.NumCaja);
                    comm.Parameters.AddWithValue("@NumSello", oSalidaEditar.NumSello);
                    comm.Parameters.AddWithValue("@Auditor", oSalidaEditar.Auditor);
                    comm.Parameters.AddWithValue("@Folio", oSalidaEditar.Folio);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
            }
        }
    }
}