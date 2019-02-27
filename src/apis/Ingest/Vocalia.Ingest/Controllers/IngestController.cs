using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vocalia.Ingest.Repositories;

namespace Ingest_API.Controllers
{
    [Route("api")]
    [ApiController]
    public class IngestController : ControllerBase
    {
        public IIngestRepository Repository { get; }

        public IngestController(IIngestRepository repository)
        {
            Repository = repository;
        }

       
    }
}
