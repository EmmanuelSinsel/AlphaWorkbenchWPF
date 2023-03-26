using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Data;
using System.Security.RightsManagement;
using System.Runtime.Intrinsics.Arm;
using System.Drawing;
using System.Data.Common;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace AlphaWorkbenchWPF
{
    public class data_manager : dbManager
    {
        public static string? ruta = MainWindow.ruta;
        public static string? raiz = MainWindow.raiz;
        public static string config_path = MainWindow.config_path;
        string imagesource = ruta + "yo.jpg";
        string imgchat = ruta + "chats_dark.png";
        string imgactivity = ruta + "activities_dark.png";
        string imghome = ruta + "home_dark.png";
        public string imgadd = ruta + "add_dark.png";


        //FUNCIONES RELACIONADAS CON EL INICIO DE SESION DEL USUARIO
        SqlConnection cnn;
        SqlConnection cnn_aux;
        SqlConnection cnn_aux1;

        public data_manager()
        {
            cnn = conection();
            cnn_aux = conection();
            cnn_aux1 = conection();
        }

        public bool login(string user, string pass)
        {
            if(get_user(user, true, "Usuario").password == pass)
            {
                return true;
            }
            return false;
        }
        public user get_user(string user, bool auth, string by)
        {
            user userprofile = new user();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Usuarios where "+by+"='" + user + "'", cnn_aux);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();

            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {
                    if (auth == true) { 
                        userprofile = new user{
                            id = rdr["ID_USuario"].ToString(),
                            user_name = rdr["Usuario"].ToString(),
                            password = rdr["Contraseña"].ToString(),
                            name = rdr["Nombre"].ToString(),
                            email = rdr["Correo"].ToString(),
                            tel = rdr["Telefono"].ToString(),
                        };
                    }
                    else if (auth == false)
                    {
                        userprofile = new user
                        {
                            id = rdr["ID_USuario"].ToString(),
                            user_name = rdr["Usuario"].ToString(),
                            password = "",
                            name = rdr["Nombre"].ToString(),
                            app = rdr["Apellido1"].ToString(),
                            apm = rdr["Apellido2"].ToString(),
                            email = rdr["Correo"].ToString(),
                            tel = rdr["Telefono"].ToString(),
                        };
                    }
                }
            }

            rdr.Close(); //Cerrar el datareader
            return userprofile;
        }
        public string get_user_image(string user)
        {
            if(!File.Exists(MainWindow.main_root + user + ".png"))
            {
                try
                {
                    byte[] image;
                    user userprofile = new user();
                    SqlCommand cmd =
                                new SqlCommand("select FotoPerfil from Usuarios where Usuario='" + user + "'", cnn_aux1);
                    image = (byte[])cmd.ExecuteScalar();
                    MemoryStream ms = new MemoryStream(image);
                    Image i = Image.FromStream(ms);
                    i.Save(MainWindow.main_root + user + ".png");
                    return MainWindow.main_root + user + ".png";
                }
                catch
                {

                }
                return ruta + "default.jpg";
            }
            else
            {
                return MainWindow.main_root + user + ".png";
            }
           
        }
        public string get_file(string id_activity)
        {
            if (!File.Exists(MainWindow.main_root + id_activity + ".pdf"))
            {
                try
                {
                    byte[] data;
                    user userprofile = new user();
                    SqlCommand cmd =
                                new SqlCommand("select Archivo from Actividades where Id_ActividadProyecto='" + id_activity + "'", cnn_aux1);
                    data = (byte[])cmd.ExecuteScalar();
                    using var stream = File.Create(MainWindow.main_root + id_activity + ".pdf");
                    stream.Write(data, 0, data.Length);
                    return MainWindow.main_root + id_activity + ".pdf";
                }
                catch(Exception e)
                {
                }
                return "Error";
            }
            else
            {
                return MainWindow.main_root + id_activity + ".pdf";
            }

        }
        public string get_project_image(string user)
        {
            if (!File.Exists(MainWindow.main_root + user + ".png"))
            {
                try
                {
                    byte[] image;
                    user userprofile = new user();
                    SqlCommand cmd =
                                new SqlCommand("select Imagen from Proyectos where Usuario='" + user + "'", cnn_aux1);
                    image = (byte[])cmd.ExecuteScalar();
                    MemoryStream ms = new MemoryStream(image);
                    Image i = Image.FromStream(ms);
                    i.Save(MainWindow.main_root + user + ".png");
                    return MainWindow.main_root + user + ".png";
                }
                catch
                {

                }
                return ruta + "default.jpg";
            }
            else
            {
                return MainWindow.main_root + user + ".png";
            }

        }
        public void generate_auto_login(string user, string pass)
        {
            using (StreamReader inputFile = new StreamReader(config_path))
            {
                string? user_id = get_user(user, false, "Usuario").id;
                inputFile.ReadLine();
                string? key = inputFile.ReadLine();
                if(key==null || key == "")
                {
                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    key = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
                    security sc = new security();
                    string ncr_pass = sc.Encrypt(pass,key,128);
                    SqlCommand cmdconsultar =
                       new SqlCommand("insert into logins(usuario,clave) values (@usuario,@clave)",cnn);
                    //Agrgar parametros
                    Console.WriteLine("usuario");
                    cmdconsultar.Parameters.Add("@usuario", SqlDbType.BigInt).Value = Convert.ToInt64(user_id);
                    Console.WriteLine("clave");
                    cmdconsultar.Parameters.Add("@clave", SqlDbType.VarChar, 50).Value = ncr_pass;
                    //Ejecutar sentencia sql contra base de datos
                    cmdconsultar.ExecuteNonQuery();
                }
                inputFile.Close();
                File.WriteAllLines(config_path, new List<string> { user,key });
            }

        }
        public string[] auto_login()
        {
            using (StreamReader inputFile = new StreamReader(config_path))
            {
                string? user = inputFile.ReadLine();
                string? user_id = get_user(user, false, "Usuario").id;
                string? key = inputFile.ReadLine();
                if (key != null && key != "" && user != null && user != "")
                {
                    security sc = new security();
                    SqlCommand cmdConsultar =
                    new SqlCommand("select clave from logins where Usuario=" + user_id + "", cnn);
                    SqlDataReader rdr = cmdConsultar.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        //Si hubo datos, Entonces mostrar la descripcion del producto
                        while (rdr.Read())
                        {
                            string dcr_pass = sc.Decrypt(rdr["clave"].ToString(),key,128);
                            if (login(user, dcr_pass) == true)
                            {
                                rdr.Close();
                                cnn.Close();
                                return new string[] {user,dcr_pass};
                            }
                        }
                    }
                    rdr.Close();
                }
                inputFile.Close();
            }
            return new string[] {};
        }
        //FUNCIONES RELACIONADAS CON EL LISTADO Y OBTENCION DE PROYECTOS Y ACTIVIDADES

        public List<project> get_projects(string user)
        {

            List<project> projects = new List<project>();
            List<profile> userprofile = get_profile(user);
            projects.Add(new project
            {
                id = "MAINMENU",
                image = imghome,
                name = "Inicio",
                desc = "Hola user"
            });
            for (int j = 0; j < userprofile.Count; j++)
            {
                string rutaImagen = "";
                SqlCommand cmdConsultar =
                        new SqlCommand("select * from Proyectos where ID_Proyecto='" + userprofile.ElementAt(j).id_project + "'", cnn_aux);
                SqlDataReader rdr = cmdConsultar.ExecuteReader();
                if (rdr.HasRows)
                {
                    //Si hubo datos, Entonces mostrar la descripcion del producto
                    while (rdr.Read())
                    {
                        byte[] data = null;
                        try
                        {
                            data = (byte[])rdr["Imagen"];
                        }
                        catch (Exception f) { }
                        try
                        {
                            if (data != null)
                            {
                                MemoryStream ms = new MemoryStream(data);
                                Image img = Image.FromStream(ms);
                                rutaImagen = MainWindow.main_root + rdr["ID_Proyecto"].ToString() + ".png";
                                img.Save(rutaImagen);

                            }
                            else
                            {
                                rutaImagen = MainWindow.ruta + "default.jpg";
                            }
                        }
                        catch { }
                        
                        project project_temp = new project
                        {
                            id = rdr["ID_Proyecto"].ToString(),
                            name = rdr["Nombre"].ToString(),
                            desc = rdr["Descripcion"].ToString(),
                            image = rutaImagen
                        };

                        for (int i = 0; i < userprofile.Count; i++)
                        {
                            if (rdr["ID_Proyecto"].ToString() == userprofile.ElementAt(i).id_project)
                            {
                                projects.Add(project_temp);
                            }
                        }
                    }
                }
                rdr.Close(); //Cerrar el datareader
            }
            projects.Add(new project
            {
                id = "ADD",
                image = imgadd
            });
            return projects;
        }
        public List<profile> get_profile(string user)
        {
            user temp_user = get_user(user, false, "Usuario");
            List<profile> profiles = new List<profile>();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Perfiles where ID_Usuario='" + temp_user.id + "'", cnn);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();
            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {

                    profile userprofile = new profile
                    {
                        id_profile = rdr["ID_Perfil"].ToString(),
                        id_project = rdr["ID_Proyecto"].ToString(),
                        charge = rdr["Cargo"].ToString(),
                        font_color = rdr["Color"].ToString()
                    };
                    profiles.Add(userprofile);
                }
            }
            rdr.Close(); //Cerrar el datareader
            return profiles;
        }
        public profile get_profile(string user, string project)
        {
            user userprofile = get_user_byid(user);
            profile profiles = new profile();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Perfiles where ID_Usuario='" + userprofile.id + "' and ID_Proyecto='"+ project +"'", cnn);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();
            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {
                    profiles = new profile
                    {
                        id_profile = rdr["ID_Perfil"].ToString(),
                        id_project = rdr["ID_Proyecto"].ToString(),
                        charge = rdr["Cargo"].ToString(),
                        font_color = rdr["Color"].ToString(),
                        admin = rdr["Admin"].ToString()
                    };
                }
            }
            rdr.Close(); //Cerrar el datareader
            return profiles;
        }

        public user get_user_byid(string user)
        {
            user temp_profile = new user();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Usuarios where ID_Usuario='" + user + "'", cnn);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();
            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {
                    temp_profile = new user
                    {
                        id = rdr["ID_USuario"].ToString(),
                        user_name = rdr["Usuario"].ToString(),
                        password = "",
                        name = rdr["Nombre"].ToString() +" "+ rdr["Apellido1"].ToString(),
                        email = rdr["Correo"].ToString(),
                        tel = rdr["Telefono"].ToString(),
                        profile_photo = rdr["FotoPerfil"].ToString()
                    };
                }
            }
            if(temp_profile.profile_photo=="" || temp_profile.profile_photo == null)
            {
                temp_profile.profile_photo = ruta + "default.jpg";
            }
            rdr.Close(); //Cerrar el datareader
            return temp_profile;
        }
        public List<profile> get_profiles(string project)
        {
            List<profile> profiles = new List<profile>();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Perfiles where ID_Proyecto='" + project + "'", cnn);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();
            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {
                    profile temp_profile = new profile
                    {
                        id_profile = rdr["ID_Perfil"].ToString(),
                        id_project = rdr["ID_Proyecto"].ToString(),
                        id_user = rdr["ID_Usuario"].ToString(),
                        charge = rdr["Cargo"].ToString(),
                        font_color = rdr["Color"].ToString(),
                        admin = rdr["Admin"].ToString()
                    };
                    profiles.Add(temp_profile);
                }
            }
            rdr.Close(); //Cerrar el datareader
            return profiles;
        }
        public List<activity> get_activities(string project, string profile)
        {
            List<activity> activities = new List<activity>();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Actividades where ID_Proyecto='" + project + "' and ID_Perfil='"+profile+"'", cnn);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();
            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {
                    activity temp_activity = new activity
                    {
                        id_profile = rdr["ID_Perfil"].ToString(),
                        id_activity =  rdr["ID_ActividadProyecto"].ToString(),
                        id_project= rdr["ID_Proyecto"].ToString(),
                        name = rdr["Nombre"].ToString(),
                        desc = rdr["Descripcion"].ToString(),
                        link = rdr["Link"].ToString(),
                        state = rdr["Estado"].ToString(),
                        start_date = rdr["FechaComienzo"].ToString(),
                        end_date = rdr["FechaCierre"].ToString(),
                        icon = imgactivity
                    };
                    activities.Add(temp_activity);
                }
            }
            rdr.Close(); //Cerrar el datareader
            return activities;
        }
        public List<chat> get_chats(string project)
        {
            List<chat> chats = new List<chat>();
            SqlCommand cmdConsultar =
                        new SqlCommand("select * from Chats where ID_Proyecto='" + project + "'", cnn);
            SqlDataReader rdr = cmdConsultar.ExecuteReader();
            if (rdr.HasRows)
            {
                //Si hubo datos, Entonces mostrar la descripcion del producto
                while (rdr.Read())
                {
                    chat temp_chat = new chat
                    {
                        id = rdr["ID_Chat"].ToString(),
                        project_id = rdr["ID_Proyecto"].ToString(),
                        name = rdr["Nombre"].ToString(),
                        icon = imgchat
                    };
                    chats.Add(temp_chat);
                }
            }
            rdr.Close(); //Cerrar el datareader
            return chats;
        }

        public void insert_testdata()
        {

        }
        //INSERTS DEL PROGRAMA
        public void insert_user(user temp_user)
        {
            string saveStaff = "INSERT INTO Usuarios(Nombre,Apellido1,Apellido2,Usuario,Contraseña,Telefono,Correo,ID_Plan)" +
                " VALUES (@Nombre,@Apellido1,@Apellido2,@Usuario,@Contraseña,@Telefono,@Correo,@ID_Plan)";


            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar,30).Value = temp_user.name;
                querySaveStaff.Parameters.Add("@Apellido1", SqlDbType.VarChar, 30).Value = temp_user.app;
                querySaveStaff.Parameters.Add("@Apellido2", SqlDbType.VarChar, 30).Value = temp_user.apm;
                querySaveStaff.Parameters.Add("@Usuario", SqlDbType.VarChar,30).Value = temp_user.user_name;
                querySaveStaff.Parameters.Add("@Contraseña", SqlDbType.VarChar, 30).Value = temp_user.password;
                querySaveStaff.Parameters.Add("@Telefono", SqlDbType.VarChar, 10).Value = temp_user.tel;
                querySaveStaff.Parameters.Add("@Correo", SqlDbType.VarChar,50).Value = temp_user.email;
                querySaveStaff.Parameters.Add("@ID_Plan", SqlDbType.BigInt).Value = temp_user.plan;
                querySaveStaff.ExecuteNonQuery();
            }
        }
        public void insert_image(string path, string user)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(path);
            string saveStaff = "UPDATE Usuarios SET FotoPerfil = @FotoPerfil WHERE Usuario = @Usuario";

            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@Usuario", SqlDbType.VarChar,30).Value = user;
                querySaveStaff.Parameters.Add("@FotoPerfil", SqlDbType.VarBinary).Value = imgdata;
                querySaveStaff.ExecuteNonQuery();
            }
        }
        public int insert_project(project temp_project)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(temp_project.image);
            string saveStaff = "INSERT INTO Proyectos(Imagen,Nombre,Descripcion) VALUES(@Imagen,@Nombre,@Descripcion) SELECT SCOPE_IDENTITY();";

            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@Imagen", SqlDbType.VarBinary).Value = imgdata;
                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar, 40).Value = temp_project.name;
                querySaveStaff.Parameters.Add("@Descripcion", SqlDbType.VarChar, int.MaxValue).Value = temp_project.desc;
                int modified = Convert.ToInt32(querySaveStaff.ExecuteScalar());
                return modified;
            }
        }
        public int insert_member(profile temp_profile)
        {
            string saveStaff = "INSERT INTO Perfiles(ID_Proyecto,ID_Usuario,Cargo,Color,Admin) VALUES(@ID_Proyecto,@ID_Usuario,@Cargo,@Color,@Admin)   SELECT SCOPE_IDENTITY();";

            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@ID_Proyecto", SqlDbType.BigInt).Value = temp_profile.id_project;
                querySaveStaff.Parameters.Add("@ID_Usuario", SqlDbType.BigInt).Value = temp_profile.id_user;
                querySaveStaff.Parameters.Add("@Cargo", SqlDbType.VarChar, 50).Value = temp_profile.charge;
                querySaveStaff.Parameters.Add("@Color", SqlDbType.VarChar, 50).Value = temp_profile.font_color;
                querySaveStaff.Parameters.Add("@Admin", SqlDbType.VarChar, 50).Value = temp_profile.admin;
                int modified = Convert.ToInt32(querySaveStaff.ExecuteScalar());
                return modified;
            }//*/
        }
        public int insert_activity(activity temp_activity)
        {
            string saveStaff = "INSERT INTO Actividades(ID_Perfil, ID_Proyecto, Nombre, Descripcion, Link, Estado, FechaComienzo, FechaCierre, Imagen) VALUES(@ID_Perfil, @ID_Proyecto, @Nombre, @Descripcion, @Link, @Estado, @FechaComienzo, @FechaCierre, @Imagen)  SELECT SCOPE_IDENTITY();";

            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@ID_Perfil", SqlDbType.BigInt).Value = temp_activity.id_profile;
                querySaveStaff.Parameters.Add("@ID_Proyecto", SqlDbType.BigInt).Value = temp_activity.id_project;
                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar, 40).Value = temp_activity.name;
                querySaveStaff.Parameters.Add("@Descripcion", SqlDbType.VarChar, int.MaxValue).Value = temp_activity.desc;
                querySaveStaff.Parameters.Add("@Link", SqlDbType.VarChar, 100).Value = temp_activity.link;
                querySaveStaff.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = temp_activity.state;
                querySaveStaff.Parameters.Add("@FechaComienzo", SqlDbType.Date).Value =temp_activity.start_date;
                querySaveStaff.Parameters.Add("@FechaCierre", SqlDbType.Date).Value = temp_activity.end_date;
                querySaveStaff.Parameters.Add("@Imagen", SqlDbType.VarChar, 100).Value = temp_activity.icon;
                int modified = Convert.ToInt32(querySaveStaff.ExecuteScalar());
                return modified;
            }
        }
        public int insert_chat(chat temp_chat)
        {
            string saveStaff = "INSERT INTO Chats(ID_Proyecto, Nombre, Imagen) VALUES(@ID_Proyecto, @Nombre, @Imagen)  SELECT SCOPE_IDENTITY();";

            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@ID_Proyecto", SqlDbType.BigInt).Value = temp_chat.project_id;
                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = temp_chat.name;
                querySaveStaff.Parameters.Add("@Imagen", SqlDbType.VarChar, 100).Value = temp_chat.icon;
                int modified = Convert.ToInt32(querySaveStaff.ExecuteScalar());
                return modified;
            }
        }
        public void upload_file(string root, string id_activity)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(root);
            SqlCommand cmd = new SqlCommand("update Actividades set Archivo = @archivo, Estado='Entregada' Where Id_ActividadProyecto = '"+id_activity+"'", cnn);
            cmd.Parameters.Add("@archivo", SqlDbType.VarBinary).Value = imgdata;
            cmd.ExecuteNonQuery();
        }
        public void update_activity(activity temp_activity)
        {
            string saveStaff = "UPDATE Actividades SET ID_Perfil=@ID_Perfil, ID_Proyecto=@ID_Proyecto, Nombre=@Nombre, Descripcion=@Descripcion, Link=@Link, FechaComienzo=@FechaComienzo, FechaCierre=@FechaCierre, Imagen=@Imagen WHERE Id_ActividadProyecto = @id_actividad";
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_actividad", SqlDbType.BigInt).Value = temp_activity.id_activity;
                querySaveStaff.Parameters.Add("@ID_Perfil", SqlDbType.BigInt).Value = temp_activity.id_profile;
                querySaveStaff.Parameters.Add("@ID_Proyecto", SqlDbType.BigInt).Value = temp_activity.id_project;
                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar, 40).Value = temp_activity.name;
                querySaveStaff.Parameters.Add("@Descripcion", SqlDbType.VarChar, int.MaxValue).Value = temp_activity.desc;
                querySaveStaff.Parameters.Add("@Link", SqlDbType.VarChar, 100).Value = temp_activity.link;
                querySaveStaff.Parameters.Add("@FechaComienzo", SqlDbType.Date).Value = temp_activity.start_date;
                querySaveStaff.Parameters.Add("@FechaCierre", SqlDbType.Date).Value = temp_activity.end_date;
                querySaveStaff.Parameters.Add("@Imagen", SqlDbType.VarChar, 100).Value = temp_activity.icon;
                querySaveStaff.ExecuteNonQuery();
            }
        }

        public void update_activity_state(string id_activity, string estado)
        {
            string saveStaff = "UPDATE Actividades SET Estado=@estado WHERE Id_ActividadProyecto = @id_actividad";
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_actividad", SqlDbType.BigInt).Value = id_activity;
                querySaveStaff.Parameters.Add("@estado", SqlDbType.VarChar).Value = estado;
                querySaveStaff.ExecuteNonQuery();
            }
        }

        public void delete_activity(string id_actividad)
        {
            string saveStaff = "DELETE FROM Actividades WHERE Id_ActividadProyecto = @id_actividad";
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_actividad", SqlDbType.BigInt).Value = id_actividad;
                querySaveStaff.ExecuteNonQuery();
            }
        }

        public void update_member(profile temp_profile)
        {
            string saveStaff = "UPDATE Perfiles SET Cargo=@Cargo,Color=@Color,Admin=@Admin WHERE ID_Perfil = @id_perfil";

            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_perfil", SqlDbType.BigInt).Value = temp_profile.id_profile;
                querySaveStaff.Parameters.Add("@Cargo", SqlDbType.VarChar, 50).Value = temp_profile.charge;
                querySaveStaff.Parameters.Add("@Color", SqlDbType.VarChar, 50).Value = temp_profile.font_color;
                querySaveStaff.Parameters.Add("@Admin", SqlDbType.VarChar, 50).Value = temp_profile.admin;
                querySaveStaff.ExecuteNonQuery();

            }//*/
        }

        public void delete_member(string id_perfil)
        {
            string saveStaff = "DELETE FROM Perfiles WHERE ID_Perfil = @id_perfil";
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_perfil", SqlDbType.BigInt).Value = id_perfil;
                querySaveStaff.ExecuteNonQuery();
            }
        }

        public void update_project(project temp_project)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes(temp_project.image);
            string saveStaff = "UPDATE Proyectos SET Imagen=@imagen,Nombre=@nombre,Descripcion=@desc WHERE ID_Proyecto = @id_proyecto";
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_proyecto", SqlDbType.BigInt).Value = temp_project.id;
                querySaveStaff.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = temp_project.name;
                querySaveStaff.Parameters.Add("@desc", SqlDbType.VarChar).Value = temp_project.desc;
                querySaveStaff.Parameters.Add("@imagen", SqlDbType.VarBinary).Value = imgdata;
                querySaveStaff.ExecuteNonQuery();
            }//*/
        }

        public void delete_project(string id_proyecto)
        {
            string saveStaff = "DELETE FROM Proyectos WHERE ID_Proyecto = @id_proyecto";
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@id_proyecto", SqlDbType.BigInt).Value = id_proyecto;
                querySaveStaff.ExecuteNonQuery();
            }
        }

        public void update_user(user temp_user)
        {
            string saveStaff = "UPDATE Usuarios SET Nombre=@Nombre,Apellido1=@Apellido1,Apellido2=@Apellido2,Usuario=@Usuario,Contraseña=@Contraseña,Telefono=@Telefono,Correo=@Correo,ID_Plan=@ID_Plan, FotoPerfil = @imagen WHERE ID_Usuario=@id_usuario";

            byte[] imgdata = System.IO.File.ReadAllBytes(temp_user.profile_photo);
            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;

                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar, 30).Value = temp_user.name;
                querySaveStaff.Parameters.Add("@Apellido1", SqlDbType.VarChar, 30).Value = temp_user.app;
                querySaveStaff.Parameters.Add("@Apellido2", SqlDbType.VarChar, 30).Value = temp_user.apm;
                querySaveStaff.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = temp_user.user_name;
                querySaveStaff.Parameters.Add("@Contraseña", SqlDbType.VarChar, 30).Value = temp_user.password;
                querySaveStaff.Parameters.Add("@Telefono", SqlDbType.VarChar, 10).Value = temp_user.tel;
                querySaveStaff.Parameters.Add("@Correo", SqlDbType.VarChar, 50).Value = temp_user.email;
                querySaveStaff.Parameters.Add("@ID_Plan", SqlDbType.BigInt).Value = temp_user.plan;
                querySaveStaff.Parameters.Add("@imagen", SqlDbType.VarBinary).Value = imgdata;
                querySaveStaff.Parameters.Add("@id_usuario", SqlDbType.BigInt).Value = temp_user.id;
                querySaveStaff.ExecuteNonQuery();
            }
        }

        public void delete_user(string id_user)
        {
            string saveStaff = "INSERT INTO Usuarios(Nombre,Apellido1,Apellido2,Usuario,Contraseña,Telefono,Correo,ID_Plan)" +
                " VALUES (@Nombre,@Apellido1,@Apellido2,@Usuario,@Contraseña,@Telefono,@Correo,@ID_Plan)";


            using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
            {
                querySaveStaff.Connection = cnn;
                querySaveStaff.Parameters.Add("@Nombre", SqlDbType.VarChar, 30).Value = id_user;
                querySaveStaff.ExecuteNonQuery();
            }
        }
    }

    //ESTRUCUTRAS DE DATOS

    public class profile
    {
        public string? id_profile { get; set; }
        public string? id_project { get; set; }
        public string? id_user { get; set; } 
        public string? charge { get; set; }
        public string? font_color { get; set; }
        public string admin { get; set; }
    }
    public class project
    {
        public string? id { get; set; }
        public string? image { get; set; }
        public string? name { get; set; }
        public string? desc { get; set; }
    }
    public class user
    {
        public string? id { get; set; }
        public string? user_name { get; set; }
        public string? password { get; set; }
        public string? name { get; set; }
        public string? app { get; set; }
        public string? apm { get; set; }
        public string? email { get; set; }
        public string? tel { get; set; }
        public string? profile_photo { get; set; }
        public string? charge { get; set; }
        public string? font_color { get; set; }
        public int? plan { get; set; }
        public string? plan_enddate { get; set; }
    }
    public class activity
    {
        public string? id_profile { get; set; }
        public string? id_activity { get; set; }
        public string? id_project { get; set; }
        public string? name { get; set; }
        public string? desc { get; set; }
        public string? link { get; set; }
        public string? state { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? icon { get; set; }
    }
    public class chat
    {
        public string? project_id { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public string? icon { get; set; }
    }
    public class list_activities
    {
        public string? project { get; set; }
        public string? name { get; set; }
        public string? photo { get; set; }
    }
}