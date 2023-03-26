using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlphaWorkbenchWPF
{

    public partial class MainWindow : Window 
    {
        animations anm = new animations();
        //RUTAS
        public static string? raiz = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName; //Pone la raiz de donde esta el proyecto
        public static string ruta = raiz + "\\Resources\\";
        public static string config_path = raiz + "\\config.CONFIG";
        public static string main_root = "C:\\AlphaWorkbenchData\\";
        //VARIABLES GLOBALES - MECANICAS
        int members_switch = 0;
        int activities_switch = 0;
        int chats_switch = 0;
        int expandMember = 0;
        int expandActivity = 0;
        int expandChat = 0;
        int menu_switch = 0;
        int exProjectWidth = 217;
        int minimizedexProjectWidth = 60;
        int window_switch = 0;
        int prev_page = 0;
        string newuserimage = "";
        int current_proyect = 0;
        //VARIABLES DE DATOS
        List<project> projects = new List<project>();
        user? user_data = new user();
        profile user_profile = new profile();
        user? add_user = new user();
        //VARIABLES GLOBALES - APARIENCIA
        int menu_font_size = 16;
        int menu_icon_size = 25;
        Brush bold = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
        Brush normal = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 50, 50, 50));
        //PARA TEMA OSCURO
        string black = "#FF000000";
        string dark_gray = "#FF404040";
        string light_gray = "#FF5D5D5D";
        //PARA TEMA CLARO
        string white = "#FFEFEFEF";
        string dark_white = "#FF5D5D5D";
        string light_dark_white = "#FF9A9999";
        string[] position_colors = { "#efd31a", "#f4ae1a", "#f26b29", "#ef4923", "#ef2f44", "#eb468a", "#b856a1", "#915ba6", "#518bca", "#27beb5", "#67bf6b", "#a2cd48" };
        public MainWindow()
        {

            InitializeComponent();
            turn_dark();
            config_visuals();
            data_manager dt = new data_manager();
            if (!File.Exists(config_path))
            {
                File.WriteAllLines(config_path, new List<string> { "", "" });
            }
            string[] login_data = dt.auto_login();
            if (login_data.Length>1)
            {
                user_login(login_data[0], login_data[1]);
                refresh_mainmenu();
            }//*/

        }
        int completedActivites = 0;
        int uncompletedActivities = 0;
        private void refresh_mainmenu()
        {
            pendantActivites.Items.Clear();
            completedActivites = 0;
            uncompletedActivities = 0;
            data_manager dt = new data_manager();
            for(int i = 0; i< projects.Count; i++)
            {
                profile temp_profile = dt.get_profile(user_data.id, projects[i].id);
                List<activity> temp_activites = new List<activity>();
                temp_activites = dt.get_activities(projects[i].id, temp_profile.id_profile);
                for(int k = 0; k < temp_activites.Count; k++)
                {
                    if (temp_activites[k].state!="Sin Comenzar")
                    {
                        completedActivites += 1;
                    }
                    else
                    {
                        uncompletedActivities += 1;
                    }
                    list_activities la = new list_activities
                    {
                        project = projects[i].name,
                        name = temp_activites[k].name,
                        photo = projects[i].image
                    };
                    pendantActivites.Items.Add(la);
                }
                
            }
            menuUncompletedCount.Text = uncompletedActivities.ToString();
            menuCompletedCount.Text = completedActivites.ToString();
            try
            {
                menuUncompletedPercent.Text = Convert.ToString((uncompletedActivities / (uncompletedActivities + completedActivites)) * 100) + "%";
                menuCompletedPercent.Text = Convert.ToString((completedActivites / (uncompletedActivities + completedActivites)) * 100) + "%";
            }
            catch
            {
                menuUncompletedPercent.Text = "0%";
                menuCompletedPercent.Text = "0%";
            }

        }

        private void config_visuals()
        {
            contextTitle.Text = "Inicio";
            Config.Visibility = Visibility.Hidden;
            menuLogin.Visibility = Visibility.Visible;
            //windowBlocker.Visibility = Visibility.Hidden;
            //Botones de la barra inician oscuros
            Minimize_app.OpacityMask = normal;  
            Maximize_app.OpacityMask = normal;
            Close_app.OpacityMask = 
            //Botones de la ventana de crear proyecto inician oscuros
            closeCreateProject.OpacityMask = normal;
            btnCreateProject.Background = scb("#FF424242");
            //Barra de proyecto inicia minimizada
            exProject.Width = 3;
            exTopbar.Width = 3;
            //Inicializar las labels de la barra de proyecto
            txtMain.Text = "Inicio";
            txtMember.Text = "Miembros";
            txtActivites.Text = "Actividades";
            txtChats.Text = "Chats";
            //DESACTIVAR SCROLL BARS DE LIST BOX TOP MENU
            exTopbar.SetValue(
            ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            exTopbar.SetValue(
            ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            //DESACTIVAR SCROLL BARS DE LIST BOX MENU
            exProject.SetValue(
            ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            exProject.SetValue(
            ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            //DESACTIVAR SCROLL BARS DE LIST BOX MIEMBROS
            exDataMembers.SetValue(
            ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            exDataMembers.SetValue(
            ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            //DESACTIVAR SCROLL BARS DE LIST BOX ACTIVIDADES
            exDataActivities.SetValue(
            ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            exDataActivities.SetValue(
            ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            //DESACTIVAR SCROLL BARS DE LIST BOX CHATS
            exDataChats.SetValue(
            ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            exDataChats.SetValue(
            ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);
            //Inicializan el tamaño de los textos
            txtMain.FontSize = menu_font_size;
            txtMember.FontSize = menu_font_size;
            txtActivites.FontSize = menu_font_size;
            txtChats.FontSize = menu_font_size;
            imgMain.Height = menu_icon_size;
            imgMembers.Height = menu_icon_size;
            imgActivities.Height = menu_icon_size;
            imgChats.Height = menu_icon_size;
            //Inicializan las imagenes
            imgActivities.Source = bimg("activities_dark.png",true);
            imgMain.Source = bimg("home_menu_dark.png", true);
            imgChats.Source = bimg("chats_dark.png", true);
            imgMembers.Source = bimg("members_dark.png", true);
            imgHome.Source = bimg("arrow_dark_rotated.png", true);
            //imgTheme.Source = bimg("theme_dark.png");
            imgUserSource.ImageSource = bimg("members_dark.png", true);
            imgAW.Source = bimg("logo.png", true);
            Minimize_app.Source = bimg("minimize.png", true);
            Maximize_app.Source = bimg("maximize.png", true);
            Close_app.Source = bimg("close.png", true);
            projectImageBuscarBrush.ImageSource = bimg("add_dark.png", true);
            closeCreateProject.Source = bimg("close.png", true);
            logoLogin.Source = bimg("aw.png", true);
            //imgLogoReg.Source = bimg("aw.png");
            logoAlert.Source = bimg("aw.png", true);
            userImageNew.ImageSource = bimg("add_dark.png", true);
            closeAddMember.Source = bimg("close.png", true);
            closeCreateActivity.Source = bimg("close.png", true);
            closeupdateCreateActivity.Source = bimg("close.png", true);
            closeupdateMember.Source = bimg("close.png", true);
            closeUpdateProject.Source = bimg("close.png", true);
            closeupdateUser.Source = bimg("close.png", true);
        }

        public BitmapImage bimg(string name, bool self)
        {
            try
            {
                //Toma la ruta de una imagen y devuelve como bitmapimage
                BitmapImage bimg = new BitmapImage();
                bimg.BeginInit();
                if (self == true)
                {
                    bimg.UriSource = new Uri(ruta + name);
                }
                if (self == false)
                {
                    bimg.UriSource = new Uri(name);
                }
                bimg.EndInit();
                return bimg;
            }
            catch { }
            return null;
            
        }
        List<profile> members = new List<profile>();
        List<activity> activites = new List<activity>();
        List<chat> chats = new List<chat>();
        private void exListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //0 dark, 1 light
        {
            if (exListBox.SelectedIndex > -1)
            {
                expandMember = 0;
                expandActivity = 0;
                expandChat = 0;
                data_manager dt = new data_manager();//Declara un objeto del tipo data_manager
                exDataActivities.Items.Clear();//Limpia los items de la lista de actividades
                exDataMembers.Items.Clear();//Limpia los items de la lista de miembros
                exDataChats.Items.Clear();//Limpia los items de la lista de chats
                project? tempPD = projects.ElementAt(exListBox.SelectedIndex);//Toma el proyecto seleccionado
                if (projects.ElementAt(exListBox.SelectedIndex).id != "MAINMENU" && projects.ElementAt(exListBox.SelectedIndex).id != "ADD")//Verifica que el proyecto seleccionado no sea la pagina principal ni la seccion de crear proyecto
                {
                    user_profile = dt.get_profile(user_data.id, tempPD.id);//Toma el perfil del usuario en el proyecto
                    members = dt.get_profiles(tempPD.id);//Toma los perfiles que estén relacionados con el proyecto
                    for (int i = 0; i <= members.Count - 1; i++)//Llena la lista de miembros
                    {
                        user temp_user = dt.get_user_byid(members.ElementAt(i).id_user);//Toma el miembro con base a su id
                        temp_user.profile_photo = dt.get_user_image(temp_user.user_name);
                        temp_user.charge = members.ElementAt(i).charge;//Asigna el cargo coorespondiente al miembro actual
                        temp_user.font_color = members.ElementAt(i).font_color;//Asigna el color del cargo al miembro actual
                        exDataMembers.Items.Add(temp_user);//Añade un item a la lista de miembros con la informacion del miembro actual
                    }
                    activites = dt.get_activities(tempPD.id, user_profile.id_profile);//Toma la lista de actividades que fueron asignadas al miembro
                    chats = dt.get_chats(tempPD.id);//Toma la lista de chats del proyecto
                    for (int i = 0; i <= activites.Count - 1; i++)
                    {
                        exDataActivities.Items.Add(activites[i]);//Añade a la lista de actividades la actividad actual relacionada con el usuario 
                    }
                    /*for (int i = 0; i <= chats.Count - 1; i++)
                    {
                        exDataChats.Items.Add(chats[i]);//Añade a la lista de chats los chats del proyecto
                    }//*/
                    //Opciones solo Visibles para los Admin
                    if (user_profile.admin == "1")
                    {
                        exDataMembers.Items.Add(new user
                        {
                            id = "ADDPROFILE",
                            profile_photo = ruta + "add_dark.png"
                        });
                        members.Add(new profile
                        {
                            id_profile = "ADDPROFILE"
                        });
                        exDataActivities.Items.Add(new activity
                        {
                            id_activity = "ADDACTIVITY",
                            icon = ruta + "add_dark.png"
                        });
                        activites.Add(new activity
                        {
                            id_activity = "ADDACTIVITY"
                        });
                        exDataChats.Items.Add(new chat
                        {
                            id = "ADDCHAT",
                            icon = ruta + "add_dark.png"
                        });
                        chats.Add(new chat
                        {
                            id = "ADDCHAT"
                        });
                    }
                    expandMember += 70 * members.Count;//Establece el tamaño de la lista de miembros desplegada
                    expandActivity += 70 * activites.Count;//Establece el tamaño de la lista de activiades desplegada
                    expandChat += 70 * chats.Count;//Establece el tamaño de la lista de chats desplegada
                    txtTitle.Text = projects.ElementAt(exListBox.SelectedIndex).name;//Pone el nombre del proyecto en la barra de titulo
                    contextTitle.Text = projects.ElementAt(exListBox.SelectedIndex).name;
                    close_all_menu(0);//Cierra todas las listas
                    anm.anim_width(exProject, exProjectWidth);//Ajusta el tamaño de la pantall principal
                    anm.anim_width(exTopbar, exProjectWidth);//'' ''
                    anm.anim_width(spacer, exProjectWidth);//'' ''
                    menu_switch = 0;//Establece el switch de menú a cerrado
                    prev_page = exListBox.SelectedIndex;
                    exProject.SelectedIndex = 0;
                }
                else if (projects.ElementAt(exListBox.SelectedIndex).id == "MAINMENU")
                {
                    if (exTopbar.Width != 3)
                    {
                        close_all_menu(0);
                        anm.anim_width(exProject, Convert.ToInt16(exProject.Width), 3);
                        anm.anim_width(exTopbar, Convert.ToInt16(exTopbar.Width), 3);
                        anm.anim_width(spacer, Convert.ToInt16(spacer.Width), 0);
                        menu_switch = 0;
                    }
                    contextTitle.Text = "Inicio";
                    clear_page();
                    pageMenu.Visibility = Visibility.Visible;
                }
                else if (projects.ElementAt(exListBox.SelectedIndex).id == "ADD")
                {
                    /*if (exTopbar.Width != 3)
                    {
                        close_all_menu(0);
                        anm.anim_width(exProject, Convert.ToInt16(exProject.Width), 3);
                        anm.anim_width(exTopbar, Convert.ToInt16(exTopbar.Width), 3);
                        anm.anim_width(spacer, Convert.ToInt16(spacer.Width), 0);
                        menu_switch = 0;
                    }*/
                    addProjectSP.Width = 0;
                    anm.anim_width(addProjectSP, 1200, 0.1);
                    contextTitle.Text = "Crear proyecto";
                    addProject.Visibility = Visibility.Visible;
                }
            }
            

        }

        private void exProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            anim_menu();
            if (spacer.Width != exProjectWidth)
            {
                anm.anim_width(spacer, exProjectWidth);
            }
            exProject.SelectedIndex = -1;
        }

        private void exDataMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            data_manager dt = new data_manager();
            clear_page();
            pageMembers.Visibility = Visibility.Visible;
            if(exDataMembers.SelectedIndex > -1)
            {
                if (members[exDataMembers.SelectedIndex].id_profile == "ADDPROFILE")
                {
                    addMember.Visibility = Visibility.Visible;
                    current_proyect = exListBox.SelectedIndex;
                }
                else
                {

                    user temp_user = dt.get_user_byid(members[exDataMembers.SelectedIndex].id_user);
                    memberProfilePhoto.ImageSource = bimg(main_root+temp_user.user_name + ".png", false);
                    lblNombreUsuarioMiembro.Text = temp_user.user_name;
                    lblNombreMiembro.Text = temp_user.name + " " + temp_user.app + " " + temp_user.apm;
                    lblCorreoMiembro.Text = temp_user.email;
                    lblTelefonoMiembro.Text = temp_user.tel;
                    if(dt.get_profile(user_data.id, projects[exListBox.SelectedIndex].id).admin == "1")
                    {
                        adminMemberView.Visibility = Visibility.Visible;
                        int pendantActivitiesMember = 0;
                        int completedActivitiesMember = 0;
                        List<activity> temp_activities = new List<activity>();
                        pendantActivitesListMember.Items.Clear();
                        temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
                        for (int i = 0; i < temp_activities.Count; i++)
                        {
                            pendantActivitesListMember.Items.Add(new list_activities { project = temp_activities[i].name, name = temp_activities[i].state, photo = ruta + "activities_dark.png" });
                            if (temp_activities[i].state == "Sin Comenzar")
                            {
                                pendantActivitiesMember += 1;
                            }
                            else
                            {
                                completedActivitiesMember += 1;
                            }
                        }
                        try
                        {
                            memberCompletedCount.Text = completedActivitiesMember.ToString();
                            memberUncompletedCount.Text = pendantActivitiesMember.ToString();
                            memberCompletedPercent.Text = Convert.ToString((completedActivitiesMember / (pendantActivitiesMember + completedActivitiesMember)) * 100) + "%";
                            memberUncompletedPercent.Text = Convert.ToString((pendantActivitiesMember / (pendantActivitiesMember + completedActivitiesMember)) * 100) + "%";
                        }catch { }

                    }
                    else
                    {
                        adminMemberView.Visibility = Visibility.Collapsed;
                    }
                }
            }
            
        }

        private void exDataActivities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbAddMember_Activity.Items.Clear();
            clear_page();
            data_manager dt = new data_manager();
            pageActivities.Visibility = Visibility.Visible;
            if (exDataActivities.SelectedIndex > -1)
            {
                if (activites[exDataActivities.SelectedIndex].id_activity == "ADDACTIVITY")
                {
                    addActivity.Visibility = Visibility.Visible;
                    for (int i = 0; i < members.Count; i++)
                    {
                        user temp_user = dt.get_user_byid(members[i].id_user);
                        cbAddMember_Activity.Items.Add(temp_user.name+" "+temp_user.app+" "+ temp_user.apm);
                    }

                }
                else
                {
                    txtActivityTittle.Text = activites[exDataActivities.SelectedIndex].name;
                    txtActivityDesc.Text = activites[exDataActivities.SelectedIndex].desc;
                    activityState.Text = activites[exDataActivities.SelectedIndex].state;
                }
            }
            
        }


        private void exDataChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clear_page();
            pageChats.Visibility = Visibility.Visible;
            if(exDataChats.SelectedIndex > -1)
            {
                if (chats[exDataChats.SelectedIndex].id == "ADDCHAT")
                {
                    addMember.Visibility = Visibility.Visible;
                }
                else
                {

                }
            }
            
        }

        private void exProject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //anim_menu();
        }

        private void anim_menu()
        {
            if (exTopbar.Width != exProjectWidth)
            {
                anm.anim_width(exProject, minimizedexProjectWidth, exProjectWidth);
                anm.anim_width(exTopbar, minimizedexProjectWidth, exProjectWidth);
                menu_switch = 0;
            }
            if (exProject.SelectedIndex == 0)
            {
                pendantActivitesListEach.Items.Clear();
                close_all_menu(0.1);
                clear_page();
                pageMain.Visibility = Visibility.Visible;
                txtMainTittle.Text = projects[exListBox.SelectedIndex].name;
                txtMainDesc.Text = projects[exListBox.SelectedIndex].desc;

                for(int i = 0; i< activites.Count-1; i++)
                {
                    list_activities ls = new list_activities
                    {
                        project = activites[i].name,
                        name = activites[i].state,
                        photo = ruta+"activities_dark.png"

                    };
                    pendantActivitesListEach.Items.Add(ls);
                }
            }
            if (exProject.SelectedIndex == 1)
            {
                if (members_switch == 0)
                {
                    anm.anim_height(exDataMembers, expandMember, 0.1);
                    members_switch = 1;
                    return;
                }
                if (members_switch == 1)
                {
                    anm.anim_height(exDataMembers, 0,0.1);
                    members_switch = 0;
                    return;
                }
                clear_page();
                pageMembers.Visibility = Visibility.Visible;

            }
            if (exProject.SelectedIndex == 2)
            {
                if (activities_switch == 0)
                {
                    anm.anim_height(exDataActivities, expandActivity,0.1);
                    activities_switch = 1;
                    return;
                }
                if (activities_switch == 1)
                {
                    anm.anim_height(exDataActivities, 0,0.1);
                    activities_switch = 0;
                    return;
                }
                clear_page();
                pageActivities.Visibility = Visibility.Visible;
            }
            if (exProject.SelectedIndex == 3)
            {
                if (chats_switch == 0)
                {
                    anm.anim_height(exDataChats, expandChat,0.1);
                    chats_switch = 1;
                    return;
                }
                if (chats_switch == 1)
                {
                    anm.anim_height(exDataChats, 0,0.1);
                    chats_switch = 0;
                    return;
                }
                clear_page();
                pageMembers.Visibility = Visibility.Visible;
            }
        }

        private void close_all_menu(double time)
        {
            anm.anim_height(exDataMembers, 0,time);
            anm.anim_height(exDataActivities, 0,time);
            anm.anim_height(exDataChats, 0,time);
            members_switch = 0;
            activities_switch = 0;
            chats_switch = 0;
        }

        private void exProject_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void exProject_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private SolidColorBrush scb (string color)
        {
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
        }

        private void turn_dark()
        {
            //CAMBIAR FONDO DE LOS CONTENEDORES
            lateralBar.Background = scb(dark_gray);
            mainGrid.Background = scb(light_gray);
            exListBox.Background = scb(light_gray);
            exListBox.BorderBrush = scb(light_gray);
            exProject.Background = scb(dark_gray);
            exProject.BorderBrush = scb(dark_gray);
            topBar.Background = scb(black);
            exTopbar.Background = scb(dark_gray);
            exTopbar.BorderBrush = scb(dark_gray);
            exListBox.Foreground = scb(white);
            exProject.Foreground = scb(white);
            exDataActivities.Foreground = scb(white);
            exDataMembers.Foreground = scb(white);
            exDataChats.Foreground = scb(white);
            txtMain.Foreground = scb(white);
            txtTitle.Foreground = scb(white);
            configAW.Foreground = scb(white);
            configGeneral.Foreground = scb(white);
            configProfile.Foreground = scb(white);
            configVisual.Foreground = scb(white);
            closeSession.Foreground = scb("DARKRED");
        }
        private void turn_light()
        {
            //CAMBIAR FONDO DE LOS CONTENEDORES
            lateralBar.Background = scb(light_dark_white);
            mainGrid.Background = scb(white);
            exListBox.Background = scb(white);
            exListBox.BorderBrush = scb(white);        
            exProject.Background = scb(light_dark_white);
            exProject.BorderBrush = scb(light_dark_white);
            topBar.Background = scb(dark_white);
            exTopbar.Background = scb(light_dark_white);
            exTopbar.BorderBrush = scb(light_dark_white);
            exListBox.Foreground = scb(black);
            exProject.Foreground = scb(black);
            exDataActivities.Foreground = scb(black);
            exDataMembers.Foreground = scb(black);
            exDataChats.Foreground = scb(black);
            txtMain.Foreground = scb(black);
            txtTitle.Foreground = scb(black);
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            
            if (menu_switch == 0)
            {
                imgHome.Source = bimg("arrow_dark.png", true);
                close_all_menu(0.1);
                anm.anim_width(exProject, exProjectWidth, minimizedexProjectWidth);
                anm.anim_width(exTopbar, exProjectWidth, minimizedexProjectWidth);
                anm.anim_width(spacer, exProjectWidth, minimizedexProjectWidth);
                menu_switch = 1;
                return;
            }
            if (menu_switch == 1)
            {
                imgHome.Source = bimg("arrow_dark_rotated.png", true);
                anm.anim_width(exProject, minimizedexProjectWidth, exProjectWidth);
                anm.anim_width(exTopbar, minimizedexProjectWidth, exProjectWidth);
                anm.anim_width(spacer, minimizedexProjectWidth, exProjectWidth);
                menu_switch = 0;
                return;
            }
            exTopbar.SelectedIndex = -2;
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(tbUser.Text!="" && tbPass.Password != "")
            {
                string user = tbUser.Text;
                string pass = tbPass.Password;
                data_manager dt = new data_manager();
                if (dt.login(user, pass) == true)
                {
                    user_login(user, pass);
                }
                else
                {
                    Error.Visibility = Visibility.Visible;
                }
            }
        }

        public void user_login(string user, string pass)
        {
            data_manager dt = new data_manager();
            if (cbRemember.IsChecked == true)
            {
                dt.generate_auto_login(user, pass);
            }
            listProjects(user);
            menuLogin.Visibility = Visibility.Visible;
            exListBox.SelectedIndex = 0;
            menuLogin.Visibility = Visibility.Hidden;
            imgUserSource.ImageSource = bimg(dt.get_user_image(user_data.user_name), false);
            refresh_mainmenu();

        }
        public void listProjects(string user)
        {
            projects.Clear();
            exListBox.Items.Clear();
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(main_root);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch { }
            data_manager dt = new data_manager();
            user_data = dt.get_user(user, false, "Usuario");
            List<project> userProjects = dt.get_projects(user);
            userProjects = dt.get_projects(user);
            for (int i = 0; i <= userProjects.Count - 1; i++)
            {
                projects.Add(userProjects[i]);
            }
            for (int i = 0; i <= userProjects.Count - 1; i++)
            {
                exListBox.Items.Add(userProjects[i]);
            }
            refresh_mainmenu();
        }
        private void topBar_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.WindowState = WindowState.Normal;
                window_switch = 0;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.DragMove();
            }  
        }

     
 
        //BOTON MINIMIZAR       
        private void Minimize_app_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Minimize_app_MouseEnter(object sender, MouseEventArgs e)
        {
            Minimize_app.OpacityMask = bold;
        }

        private void Minimize_app_MouseLeave(object sender, MouseEventArgs e)
        {
            Minimize_app.OpacityMask = normal;
        }

        //BOTON MAXIMIZAR/NORMAL
        private void Maximize_app_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (window_switch == 1 || this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                window_switch = 0;
                return;
            }
            if (window_switch == 0)
            {
                this.WindowState = WindowState.Maximized;
                window_switch = 1;
                return;
            }
            
        }
        private void Maximize_app_MouseEnter(object sender, MouseEventArgs e)
        {
            Maximize_app.OpacityMask = bold;
        }
        private void Maximize_app_MouseLeave(object sender, MouseEventArgs e)
        {
            Maximize_app.OpacityMask = normal;
        }

        //BOTON CERRAR
        private void Close_app_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(main_root);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch { }

            System.Windows.Application.Current.Shutdown();
        }
        private void Close_app_MouseEnter(object sender, MouseEventArgs e)
        {
            Close_app.OpacityMask = bold;
        }
        private void Close_app_MouseLeave(object sender, MouseEventArgs e)
        {
            Close_app.OpacityMask = normal;
        }

        private void configReturn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Config.Visibility = Visibility.Hidden;
            if (exListBox.SelectedIndex >= 0)
            {
                contextTitle.Text = projects.ElementAt(exListBox.SelectedIndex).name;
            }
            else
            {
                contextTitle.Text = "Test";
            }

        }

        private void clear_page()
        {
            pageMenu.Visibility = Visibility.Collapsed;
            pageMain.Visibility = Visibility.Collapsed;
            pageMembers.Visibility = Visibility.Collapsed;
            pageActivities.Visibility = Visibility.Collapsed;
            pageChats.Visibility = Visibility.Collapsed;
        }



        //FUNCIONES VENTANA CREAR PROYECTO
        private void closeCreateProject_MouseEnter(object sender, MouseEventArgs e)
        {
            closeCreateProject.OpacityMask = bold;
        }

        private void closeCreateProject_MouseLeave(object sender, MouseEventArgs e)
        {
            closeCreateProject.OpacityMask = normal;
        }

        private void closeCreateProject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtAddProjectName.Text = "";
            txtAddProjectDesc.Document.Blocks.Clear();
            exListBox.SelectedIndex = prev_page;
            addProject.Visibility = Visibility.Hidden;
            
        }

        private void btnCreateProject_MouseEnter(object sender, MouseEventArgs e)
        {
            btnCreateProject.Background = scb("#FF666666");
        }

        private void btnCreateProject_MouseLeave(object sender, MouseEventArgs e)
        {
            btnCreateProject.Background = scb("#FF424242");
        }

        private void btnCreateProject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (newprojectimage == "")
            {
                newprojectimage = ruta + "default.png";
            }
            if (txtAddProjectName.Text!="" && new TextRange(txtAddProjectDesc.Document.ContentStart, txtAddProjectDesc.Document.ContentEnd).Text != "")
            {
                project temp_project = new project
                {
                    name = txtAddProjectName.Text,
                    desc = new TextRange(txtAddProjectDesc.Document.ContentStart, txtAddProjectDesc.Document.ContentEnd).Text,
                    image = newprojectimage
                };
                data_manager dt = new data_manager();
                current_proyect = dt.insert_project(temp_project);
                addProject.Visibility = Visibility.Hidden;
                addProjectAdminRole.Visibility = Visibility.Visible;
                exListBox.SelectedIndex = -1;
                exListBox.Items.RemoveAt(exListBox.Items.Count - 1);
                projects.RemoveAt(projects.Count - 1);
                exListBox.Items.Add(temp_project);
                exListBox.Items.Add(new project
                {
                    id = "ADD",
                    image = dt.imgadd
                });
                temp_project.id = current_proyect.ToString();
                projects.Add(temp_project);
                projects.Add(new project
                {
                    id = "ADD",
                    image = dt.imgadd
                });
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }

        }

        private void btnAddMemberAdmin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtPositionAdmin.Text != "" && adminColorPicker.SelectedIndex!=-1)
            {
                profile temp_profile = new profile
                {
                    id_user = user_data.id,
                    id_project = current_proyect.ToString(),
                    charge = txtPositionAdmin.Text,
                    font_color = position_colors[adminColorPicker.SelectedIndex],
                    admin = "1"
                };
                data_manager dt = new data_manager();
                dt.insert_member(temp_profile);
                addProjectAdminRole.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            menuLogin.Visibility = Visibility.Hidden;
            menuRegister.Visibility = Visibility.Visible;
        }

        private void btnCancelReg_Click(object sender, RoutedEventArgs e)
        {
            menuLogin.Visibility = Visibility.Visible;
            menuRegister.Visibility = Visibility.Hidden;
        }

        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            data_manager dt = new data_manager();//Declara un objeto del tipo data_manager
            if (newuserimage == "")
            {
                newuserimage = ruta+"default.png";
            }
            if (tbRegUser.Text!="" && tbRegMail.Text != "" && tbRegPass.Password!="" && tbRegPassConfirm.Password != "" && tbRegName.Text != "" && tbRegAPP.Text != "" && tbRegAPM.Text != "" && tbRegTel.Text != "")
            {
                if (dt.get_user(tbRegMail.Text, false, "Correo").user_name == null)
                {
                    if (tbRegPass.Password == tbRegPassConfirm.Password)
                    {
                        dt.insert_user(new user
                        {
                            user_name = tbRegUser.Text,
                            password = tbRegPass.Password,
                            name = tbRegName.Text,
                            app = tbRegAPP.Text,
                            apm = tbRegAPM.Text,
                            email = tbRegMail.Text,
                            tel = tbRegTel.Text,
                            plan = 1,
                        });
                        dt.insert_image(newuserimage, tbRegUser.Text);
                        RegisteredUserAlert.Visibility = Visibility.Visible;
                        menuRegister.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            RegisteredUserAlert.Visibility = Visibility.Hidden;
            menuLogin.Visibility = Visibility.Visible;
        }

        private void addUserImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "All Files (*.*) | *.*",
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() != DialogResult.HasValue)
                newuserimage = "";
            //Return the file the user selected
            newuserimage = dlg.FileName;
            if (newuserimage != "")
            {
                userImageNew.ImageSource = bimg(newuserimage, false);
            }

        }

        private void btnAddMember_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            if(txtPosition.Text!=""&& positionColor.SelectedIndex != -1)
            {
                profile temp_profile = new profile
                {
                    id_user = add_user.id,
                    id_project = projects[current_proyect].id,
                    charge = txtPosition.Text,
                    font_color = position_colors[positionColor.SelectedIndex],
                    admin = "0"
                };
                if (cbAdmin.IsChecked == true)
                {
                    temp_profile.admin = "1";
                }
                dt.insert_member(temp_profile);
                addMember.Visibility = Visibility.Hidden;
                listProjects(user_data.user_name);
                exListBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }
        }

        private void btnBuscarUsuario_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            if (txtBuscarUsuario.Text != "")
            {
                add_user = dt.get_user(txtBuscarUsuario.Text, false, "Correo");
                lblBuscarNombreUsuarioUser.Text = add_user.user_name;
                if (add_user.apm != null)
                {
                    lblBuscarNombreUser.Text = add_user.name + " " + add_user.app + " " + add_user.apm;
                }
                else
                {
                    lblBuscarNombreUser.Text = add_user.name + " " + add_user.app;
                }
                lblBuscarCorreoUser.Text = add_user.email;
                lblBuscarTelefonoUser.Text = add_user.tel;

                userImageBuscarBrush.ImageSource = bimg(dt.get_user_image(add_user.user_name), false);
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }
        }
        string newprojectimage = "";

        private void projectImageBuscar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "All Files (*.*) | *.*",
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() != DialogResult.HasValue)
                newuserimage = "";
            //Return the file the user selected
            newprojectimage = dlg.FileName;
            if (newprojectimage != "")
            {
                projectImageBuscarBrush.ImageSource = bimg(newprojectimage, false);
            }
        }

        private void btnCreateActivity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            if(txtAddActivityName.Text!="" && new TextRange(txtAddActivityDesc.Document.ContentStart, txtAddActivityDesc.Document.ContentEnd).Text!="")
            {
                activity temp_activity = new activity
                {
                    id_profile = members[cbAddMember_Activity.SelectedIndex].id_profile,
                    id_project = projects[exListBox.SelectedIndex].id,
                    name = txtAddActivityName.Text,
                    desc = new TextRange(txtAddActivityDesc.Document.ContentStart, txtAddActivityDesc.Document.ContentEnd).Text,
                    link = "",
                    state = "Sin Comenzar",
                    start_date = DateTime.Today.ToString(),
                    end_date = DateTime.Today.ToString(),
                    icon = ""
                };
                dt.insert_activity(temp_activity);
                addActivity.Visibility = Visibility.Hidden;
                listProjects(user_data.user_name);
                exListBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }

        }

        private void closeAddMember_MouseDown(object sender, MouseButtonEventArgs e)
        {
            addMember.Visibility = Visibility.Hidden;
        }

        private void closeCreateActivity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            addActivity.Visibility = Visibility.Hidden;
        }

        string root="";

        private void btnUploadActivity_MouseDown(object sender, MouseButtonEventArgs e)
        {

            
            var dlg = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "All Files (*.*) | *.*",
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() != DialogResult.HasValue)
                root = "";
            //Return the file the user selected
            root = dlg.FileName;

        }

        private void btnUpload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            if (root != "")
            {
                try
                {
                    dt.upload_file(root, activites[exDataActivities.SelectedIndex].id_activity);
                    activityState.Text = "Entregada";
                }
                catch(Exception f)
                {
                    MessageBox.Show("Error al subir el archivo, intentelo de nuevo "+f.ToString());
                }

            }

        }

        private void btnDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pendantActivitesListMember.SelectedIndex != -1)
            {
                data_manager dt = new data_manager();
                List<activity> temp_activites = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
                string ruta = dt.get_file(temp_activites[pendantActivitesListMember.SelectedIndex].id_activity);
                if (ruta != "Error")
                {
                    Process.Start("explorer.exe", @ruta);
                }
                else
                {
                    MessageBox.Show("Actividad Inexistente");
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una actividad primero");
            }
        }

        private void btUpdateActivity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            List<activity> temp_activities = new List<activity>();
            temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
            if (cbupdateMember_Activity.SelectedIndex != -1 && txtupdateActivityName.Text != "" && new TextRange(txtupdateActivityDesc.Document.ContentStart, txtupdateActivityDesc.Document.ContentEnd).Text != "")
            {
                activity temp_activity = new activity
                {
                    id_activity = temp_activities[pendantActivitesListMember.SelectedIndex].id_activity,
                    id_profile = members[cbupdateMember_Activity.SelectedIndex].id_profile,
                    id_project = projects[exListBox.SelectedIndex].id,
                    name = txtupdateActivityName.Text,
                    desc = new TextRange(txtupdateActivityDesc.Document.ContentStart, txtupdateActivityDesc.Document.ContentEnd).Text,
                    link = "",
                    start_date = DateTime.Today.ToString(),
                    end_date = DateTime.Today.ToString(),
                    icon = ""
                };
                dt.update_activity(temp_activity);
                updateActivity.Visibility = Visibility.Hidden;
                listProjects(user_data.user_name);
                exListBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }

        }

        private void closeupdateCreateActivity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updateActivity.Visibility = Visibility.Hidden;
        }

        private void btnUpdate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cbupdateMember_Activity.Items.Clear();
                data_manager dt = new data_manager();
                List<activity> temp_activities = new List<activity>();
                temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
                updateActivity.Visibility = Visibility.Visible;
                txtupdateActivityName.Text = temp_activities[pendantActivitesListMember.SelectedIndex].name;
                txtupdateActivityDesc.Document.Blocks.Add(new Paragraph(new Run(temp_activities[pendantActivitesListMember.SelectedIndex].desc)));
                List<user> temp_users = new List<user>();
                for (int i = 0; i < members.Count; i++)
                {
                    user temp_user = dt.get_user_byid(members[i].id_user);
                    temp_users.Add(temp_user);
                    cbupdateMember_Activity.Items.Add(temp_user.name + " " + temp_user.app + " " + temp_user.apm);
                }
                user temp = dt.get_user_byid(members[pendantActivitesListMember.SelectedIndex].id_user);
                cbupdateMember_Activity.Text = temp.name + " " + temp.app + " " + temp.apm;
            }
            catch { }

        }

        private void btDeleteActivity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            List<activity> temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
            dt.delete_activity(temp_activities[pendantActivitesListMember.SelectedIndex].id_activity);
            updateActivity.Visibility = Visibility.Hidden;
            listProjects(user_data.user_name);
            exListBox.SelectedIndex = 0;
        }

        private void closeupdateMember_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updateMember.Visibility = Visibility.Hidden;
        }

        private void btnDeleteMember_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            dt.delete_member(members[exDataMembers.SelectedIndex].id_profile);
            updateMember.Visibility = Visibility.Hidden;
            listProjects(user_data.user_name);
            exListBox.SelectedIndex = 0;
        }

        private void btnUpdateMember_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            if (txtUpdatePosition.Text != "" && UpdatePositionColor.SelectedIndex!=-1)
            {
                profile temp_profile = new profile
                {
                    id_profile = members[exDataMembers.SelectedIndex].id_profile,
                    charge = txtUpdatePosition.Text,
                    font_color = position_colors[UpdatePositionColor.SelectedIndex]
                };
                if (cbUpdateAdmin.IsChecked == true)
                {
                    temp_profile.admin = "1";
                }
                else
                {
                    temp_profile.admin = "0";
                }
                dt.update_member(temp_profile);
                updateMember.Visibility = Visibility.Hidden;
                listProjects(user_data.user_name);
                exListBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }

        }

        private void btnUpdateUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updateMember.Visibility = Visibility.Visible;
            lblUpdateNombreUsuarioUser.Text = lblNombreUsuarioMiembro.Text;
            lblUpdateNombreUser.Text = lblNombreMiembro.Text;
            lblUpdateCorreoUser.Text = lblCorreoMiembro.Text;
            lblUpdateTelefonoUser.Text = lblTelefonoMiembro.Text;
            txtUpdatePosition.Text = members[exDataMembers.SelectedIndex].charge;
            UpdatePositionColor.SelectedIndex = Array.IndexOf(position_colors,members[exDataMembers.SelectedIndex].charge);
            if (members[exDataMembers.SelectedIndex].admin == "1")
            {
                cbUpdateAdmin.IsChecked = true;
            }
            else
            {
                cbUpdateAdmin.IsChecked = false;
            }
        }

        private void btnUpdateProject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updateProject.Visibility = Visibility.Visible;
            projectImageUpdateBrush.ImageSource = bimg(main_root+projects[exListBox.SelectedIndex].id + ".png", false);
            txtUpdateProjectName.Text = projects[exListBox.SelectedIndex].name;
            txtUpdateProjectDesc.Document.Blocks.Add(new Paragraph(new Run(projects[exListBox.SelectedIndex].desc)));
        }

        private void closeUpdateProject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updateProject.Visibility = Visibility.Hidden;
        }

        private void projectImageUpdate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "All Files (*.*) | *.*",
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() != DialogResult.HasValue)
                newuserimage = "";
            //Return the file the user selected
            newprojectimage = dlg.FileName;
            if (newprojectimage != "")
            {
                projectImageUpdateBrush.ImageSource = bimg(newprojectimage, false);
            }
        }

        private void btnUpdateProject_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            if (newprojectimage == "")
            {
                newprojectimage = main_root + projects[exListBox.SelectedIndex].id + ".png";
            }
            if(txtUpdateProjectName.Text!="" && new TextRange(txtUpdateProjectDesc.Document.ContentStart, txtUpdateProjectDesc.Document.ContentEnd).Text != "")
            {
                project temp_project = new project
                {
                    id = projects[exListBox.SelectedIndex].id,
                    name = txtUpdateProjectName.Text,
                    desc = new TextRange(txtUpdateProjectDesc.Document.ContentStart, txtUpdateProjectDesc.Document.ContentEnd).Text,
                    image = newprojectimage
                };
                dt.update_project(temp_project);
                updateProject.Visibility = Visibility.Hidden;
                listProjects(user_data.user_name);
                exListBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }

        }

        private void btnDeleteProject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            dt.delete_project(projects[exListBox.SelectedIndex].id);
            updateProject.Visibility = Visibility.Hidden;
            listProjects(user_data.user_name);
            exListBox.SelectedIndex = 0;
        }

        //CONFIGURACION
        private void imgUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            //Config.Visibility = Visibility.Visible;
            //contextTitle.Text = "Configuracion";
            userImageUpdate.ImageSource = bimg(main_root + user_data.user_name + ".png", false);
            menuUpdate.Visibility = Visibility.Visible;
            tbUpdateUser.Text = user_data.user_name;
            tbUpdateName.Text = user_data.name;
            tbUpdateAPP.Text = user_data.app;
            tbUpdateAPM.Text = user_data.apm;
            tbUpdateMail.Text = user_data.email;
            tbUpdateTel.Text = user_data.tel;
            tbUpdatePass.Password = dt.get_user(user_data.user_name, true, "Usuario").password;
            tbUpdatePassConfirm.Password = dt.get_user(user_data.user_name, true, "Usuario").password;
        }

        private void btnUpdUser_MouseDown(object sender, MouseButtonEventArgs e)
        {

            
        }
        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);

            try
            {
                arrLine[line_to_edit - 1] = newText;
                File.WriteAllLines(fileName, arrLine);
            }
            catch { }

        }
        private void btnDelUser_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void closeupdateUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            newuserimage = "";
            menuUpdate.Visibility = Visibility.Hidden;
        }

        private void updateUserImage_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var dlg = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "All Files (*.*) | *.*",
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() != DialogResult.HasValue)
                newuserimage = "";
            //Return the file the user selected
            newuserimage = dlg.FileName;
            if (newprojectimage != "")
            {
                projectImageUpdateBrush.ImageSource = bimg(newprojectimage, false);
            }
        }

        private void btnUpdUser_Click(object sender, RoutedEventArgs e)
        {
            data_manager dt = new data_manager();//Declara un objeto del tipo data_manager
            if (newuserimage == "")
            {
                newuserimage = main_root + user_data.user_name + ".png";
            }
            if (tbUpdateUser.Text != "" && tbUpdateMail.Text != "" && tbUpdatePass.Password != "" && tbUpdatePassConfirm.Password != "" && tbUpdateName.Text != "" && tbUpdateAPP.Text != "" && tbUpdateAPM.Text != "" && tbUpdateTel.Text != "")
            {
                if (tbUpdatePass.Password == tbUpdatePassConfirm.Password)
                {
                    dt.update_user(new user
                    {
                        id = user_data.id,
                        user_name = tbUpdateUser.Text,
                        password = tbUpdatePass.Password,
                        name = tbUpdateName.Text,
                        app = tbUpdateAPP.Text,
                        apm = tbUpdateAPM.Text,
                        email = tbUpdateMail.Text,
                        tel = tbUpdateTel.Text,
                        plan = 1,
                        profile_photo = newuserimage
                    });
                }
                lineChanger(tbUpdateUser.Text, config_path, 1);
                menuUpdate.Visibility = Visibility.Hidden;
                listProjects(user_data.user_name);
                exListBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Campos vacíos o Invalidos");
            }


        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(@config_path, string.Empty);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void btnAccept_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            List<activity> temp_activities = new List<activity>();
            temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
            dt.update_activity_state(temp_activities[pendantActivitesListMember.SelectedIndex].id_activity, "Aceptada");
            update_list();

        }

        private void btndeny_MouseDown(object sender, MouseButtonEventArgs e)
        {
            data_manager dt = new data_manager();
            List<activity> temp_activities = new List<activity>();
            temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
            dt.update_activity_state(temp_activities[pendantActivitesListMember.SelectedIndex].id_activity, "Rechazada");
            update_list();
        }

        public void update_list()
        {
            data_manager dt = new data_manager();
            List<activity> temp_activities = new List<activity>();
            pendantActivitesListMember.Items.Clear();
            temp_activities = dt.get_activities(projects[exListBox.SelectedIndex].id, members[exDataMembers.SelectedIndex].id_profile);
            for (int i = 0; i < temp_activities.Count; i++)
            {
                pendantActivitesListMember.Items.Add(new list_activities { project = temp_activities[i].name, name = temp_activities[i].state, photo = ruta + "activities_dark.png" });
            }
        }
    }
}
