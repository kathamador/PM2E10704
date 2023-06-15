using PM2E10704.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PM2E10704.Controller
{
    public class DataBase
    {

        readonly SQLiteAsyncConnection dbase;

        public DataBase(string dbpath)
        {
            dbase = new SQLiteAsyncConnection(dbpath);

            dbase.CreateTableAsync<Models.Lugares>();

        }

        public Task<int> SitioSave(Lugares sitio)
        {
            if (sitio.id != 0)
            {
                return dbase.UpdateAsync(sitio);
            }
            else
            {
                return dbase.InsertAsync(sitio);
            }

        }

        public Task<List<Lugares>> getListSitio()
        {
            return dbase.Table<Lugares>().ToListAsync();
        }

        public async Task<Lugares> getSitio(int pid)
        {
            return await dbase.Table<Lugares>()
                .Where(i => i.id == pid)
                .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteSitio(Lugares sitio)
        {
            return await dbase.DeleteAsync(sitio);
        }
    }
}
