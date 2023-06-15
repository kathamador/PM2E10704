using Plugin.Media;
using PM2E10704.Views;
using PM2E10704;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM2E10704
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoadCoord()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();

                // Solicitar permisos de ubicación
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                    if (location != null)
                    {
                        var lon = location.Longitude;
                        var lat = location.Latitude;

                        txtLat.Text = lat.ToString();
                        txtLon.Text = lon.ToString();
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "No se pudo obtener la ubicación", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Advertencia", "Sin permisos de ubicación", "OK");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Advertencia", "Error de Dispositivo, Validar si su GPS esta activo", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Ubicacion" + pEx, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "OK");
            }
        }


        Plugin.Media.Abstractions.MediaFile Filefoto = null;

        private Byte[] ConvertImageToByteArray()
        {
            if (Filefoto != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = Filefoto.GetStream();
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
            return null;

        }
        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            if (Filefoto == null)
            {
                await this.DisplayAlert("Advertencia", "Debe tomar una foto", "OK");
            }
            else if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                await this.DisplayAlert("Advertencia", "Campo descripcion vacio, Ingrese datos.", "OK");
            }
            else if (string.IsNullOrEmpty(txtLat.Text) && string.IsNullOrEmpty(txtLon.Text))
            {
                await this.DisplayAlert("Advertencia", "Error al registrar. Sin coordenadas identificadas.", "OK");
                LoadCoord();
            }
            else
            {
                var sitio = new Models.Lugares
                {
                    id = 0,
                    latitud = txtLat.Text,
                    longitud = txtLon.Text,
                    descripcion = txtDescripcion.Text,
                    foto = ConvertImageToByteArray(),
                };

                var result = await App.DBase.SitioSave(sitio);

                if (result > 0)
                {
                    await DisplayAlert("Aviso", "Sitio Registrado", "OK");
                    Clear();
                }
                else
                {
                    await DisplayAlert("Aviso", "Error al Registrar", "OK");
                }
            }
        }


        private async void btnList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListLugares());
        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            //var
            Filefoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "MisFotos",
                Name = "test.jpg",
                SaveToAlbum = true,
            });


            // await DisplayAlert("path directorio", Filefoto.Path, "ok");

            if (Filefoto != null)
            {
                fotoSitio.Source = ImageSource.FromStream(() =>
                {
                    return Filefoto.GetStream();
                });
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();

                // Solicitar permisos de ubicación
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                    if (location != null)
                    {
                        var lon = location.Longitude;
                        var lat = location.Latitude;

                        txtLat.Text = lat.ToString();
                        txtLon.Text = lon.ToString();
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "No se pudo obtener la ubicación", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Advertencia", "Sin permisos de ubicación", "OK");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Advertencia", "Error de dispositivo, validar si su GPS está activo", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Ubicacion" + pEx, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "OK");
            }
        }


        private void btnSalir_Clicked(object sender, EventArgs e)
        {

            System.Environment.Exit(0);
        }

        private void Clear()
        {
            txtLat.Text = "";
            txtLon.Text = "";
            txtDescripcion.Text = "";
            //fotoSitio = null;
        }
    }
}
