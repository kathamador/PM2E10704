using PM2E10704.Models;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E10704.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListLugares : ContentPage
    {
        private Lugares selectedSitio; // Variable para almacenar el sitio seleccionado
        //public byte[] foto { get; set; }
        private bool registroEliminado = false;

        public ListLugares()
        {
            InitializeComponent();

            btnDel.Clicked += Eliminar_Clicked;
            btnMapa.Clicked += IrMapa_Clicked;
        }

        private async void Cargar_Sitios()
        {
            var sitios = await App.DBase.getListSitio();
            Lista.ItemsSource = sitios;
        }

        private void Lista_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedSitio = (Lugares)e.SelectedItem; // Almacenar el sitio seleccionado
        }

        private async void Lista_ItemHolding(ItemHoldingEventArgs e)
        {
            selectedSitio = (Lugares)e.ItemData; // Almacenar el sitio seleccionado

            var action = await DisplayActionSheet("Opciones", "Cancelar", null, "Eliminar", "Ver en el mapa");

            switch (action)
            {
                case "Eliminar":
                    await EliminarSitio();
                    break;
                case "Ver en el mapa":
                    await VerMapa();
                    break;
            }
        }
        private bool confirmacionMostrada = false; // Variable para controlar si se ha mostrado el cuadro de diálogo de confirmación

        private async Task EliminarSitio()
        {
            if (!confirmacionMostrada)
            {
                confirmacionMostrada = true; // Marcar que se ha mostrado la confirmación

                bool answer = await DisplayAlert("Confirmación", "¿Quiere eliminar el registro?", "Si", "No");
                Debug.WriteLine("Answer: " + answer);

                if (answer)
                {
                    var result = await App.DBase.DeleteSitio(selectedSitio);

                    if (result == 1)
                    {
                        await DisplayAlert("Aviso", "Registro Eliminado", "OK");
                        Cargar_Sitios();
                    }
                }

                confirmacionMostrada = false; // Restaurar el estado de la variable para permitir futuras confirmaciones
            }
        }
        private bool verMapaMostrado = false; // Variable para controlar si el mapa ya se ha mostrado

        private async Task VerMapa()
        {
            if (!verMapaMostrado)
            {
                verMapaMostrado = true; // Marcar que el mapa se ha mostrado

                bool answer = await DisplayAlert("AVISO", "¿Quiere ir al mapa?", "Si", "No");
                Debug.WriteLine("Answer: " + answer);

                if (answer)
                {
                    try
                    {
                        var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                        var tokendecancelacion = new System.Threading.CancellationTokenSource();
                        var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                        if (location != null)
                        {
                            Map map = new Map();
                            map.BindingContext = selectedSitio;
                            await Navigation.PushAsync(map);
                        }
                        Map mapp = new Map(selectedSitio.foto);
                        mapp.BindingContext = selectedSitio;
                        await Navigation.PushAsync(mapp);
                    }
                    catch (FeatureNotSupportedException fnsEx)
                    {
                        await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "Ok");
                    }
                    catch (FeatureNotEnabledException fneEx)
                    {
                        await DisplayAlert("Advertencia", "Error de Dispositivo, validar si su GPS esta activo", "Ok");
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                    catch (PermissionException pEx)
                    {
                        await DisplayAlert("Advertencia", "Sin Permisos de Geolocalizacion" + pEx, "Ok");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "Ok");
                    }
                }

                verMapaMostrado = false; // Restaurar el estado de la variable después de mostrar el mapa
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Cargar_Sitios();
        }

        private async void Eliminar_Clicked(object sender, EventArgs e)
        {
            if (selectedSitio != null)
            {
                await EliminarSitio();
            }
            else
            {
                await DisplayAlert("Advertencia", "Seleccione un elemento de la lista.", "OK");
            }
        }

        private async void IrMapa_Clicked(object sender, EventArgs e)
        {
            if (selectedSitio != null)
            {
                await VerMapa();
            }
            else
            {
                await DisplayAlert("Advertencia", "Seleccione un elemento de la lista.", "OK");
            }
        }
    }
}