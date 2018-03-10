using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Sjotterbak.WebApi.Services
{
    public class DatabaseService
    {
        private RecordsReaderWriter _readerWriter;
        private readonly string _filePath;


        public DatabaseService(IHostingEnvironment environment)
        {
            string webRootPath = environment.WebRootPath;
            _filePath = $@"{webRootPath}\App_Data\";
        }

        public Records Records()
        {
            if (_readerWriter == null)
            {
                _readerWriter = new RecordsReaderWriter(_filePath);
            }
            return _readerWriter.Records;
        }

        

        public void Persist()
        {
            _readerWriter?.Persist();
        }
    }
}
