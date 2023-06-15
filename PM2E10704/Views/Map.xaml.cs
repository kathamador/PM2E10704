using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;


namespace PM2E10704.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Map : ContentPage
    {
        public Map()
        {
            InitializeComponent();
        }
        private byte[] Filefot; // Variable para almacenar la foto recibida

        public Map(byte[] foto)
        {
            InitializeComponent();
            Filefot = foto; // Asignar la foto recibida a la variable Filefoto
        }

        Plugin.Media.Abstractions.MediaFile Filefoto = null;

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                if (location != null)
                {

                    var lat = Convert.ToDouble(mtxtLat.Text);
                    var lon = Convert.ToDouble(mtxtLon.Text);
                    var Nomsitio = nomSitio.Text;

                    var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        var geocodeAddress =
                            $"Pais: {placemark.CountryName}\n" +
                            $"Depto: {placemark.AdminArea}\n" +
                            $"Ciudad: {placemark.SubAdminArea}\n" +
                            $"Barrio o Colonia: {placemark.Locality}\n" +
                            $"Direccion: {placemark.Thoroughfare}\n";

                       await DisplayAlert("Ubicacion", geocodeAddress, "ok");

                        var pin = new Pin()
                        {
                            Position = new Position(lat, lon),
                            Label = Nomsitio,
                        };
                        Mapa.Pins.Add(pin);
                        Mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lat, lon), Distance.FromMeters(100.00)));
                    }
                }
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
                await DisplayAlert("Advertencia", "Sin Permisos de Ubicación" + pEx, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicación" + ex, "Ok");
            }
        }

        private async void btn_Compartir_Clicked(object sender, EventArgs e)
        {
            var geocodeAddress = "";

            var lat = Convert.ToDouble(mtxtLat.Text);
            var lon = Convert.ToDouble(mtxtLon.Text);
            var Nomsitio = nomSitio.Text;

            var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                geocodeAddress =
               $"\nPais: {placemark.CountryName}, \n" +
               $"Depto: {placemark.AdminArea}, \n" +
               $"Ciudad: {placemark.SubAdminArea},\n" +
               $"Colonia: {placemark.Locality},\n" +
               $"Direccion: {placemark.Thoroughfare},\n";
            }

            try
            {
                int imageWidth = 300;
                int imageHeight = 200;

                // Convertir el arreglo de bytes en una imagen base64
                var base64Image = Convert.ToBase64String(Filefot);

                // Crear el contenido HTML
                var htmlContent =
                    "<html>" +
                    "<body>" +
                    $"<h2>Lugar: {Nomsitio}</h2>" +
                    geocodeAddress +
                    $"<p>URL: <a href='https://maps.google.com/?q={lon},{lat}'>Ver en Google Maps</a></p>" +
                    $"<img src='data:image/jpeg;base64,{base64Image}' alt='Foto' style='width: {imageWidth}px; height: {imageHeight}px;'" +
                    "</body>" +
                    "</html>";

                // Crear el archivo temporal con el contenido HTML
                var tempFile = Path.Combine(FileSystem.CacheDirectory, "InformacionSitio.html");
                File.WriteAllText(tempFile, htmlContent);

                // Compartir el archivo
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartiendo Ubicación y Foto",
                    File = new ShareFile(tempFile)
                });

            }
            catch{}
        }
        private async Task<Stream> GetImageStreamAsync(ImageSource imageSource)
        {
            var imageStream = await (imageSource as StreamImageSource)?.Stream(CancellationToken.None);
            return imageStream;
        }
    }
}