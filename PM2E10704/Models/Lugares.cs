using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace PM2E10704.Models
{
    public class Lugares
    {

        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public byte[] foto { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string descripcion { get; set; }
    }
}
