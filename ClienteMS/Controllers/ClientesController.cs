using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClienteMS.Models;

namespace ClienteMS.Controllers
{
    [Route("api/clientes")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private static int _counter = 0;
        private static string _localhost;
        private static string _kernel;
        private static string _target_framework;
        private readonly ILogger<ClientesController> _logger;        
        private readonly ClienteContext _context;

        public ClientesController(ClienteContext context, ILogger<ClientesController> logger, 
            IConfiguration configuration)
        {
            System.Threading.Interlocked.Increment(ref _counter);
            _localhost = Environment.MachineName;
            _kernel = Environment.OSVersion.VersionString;
            _target_framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;

            _logger = logger;
            _context = context;
        }

        public void AddHttpHeaders(HttpResponse Response)
        {
            Response.Headers.Add("X-Total-Count", _counter.ToString());
            Response.Headers.Add("X-Localhost", _localhost.ToString());
            Response.Headers.Add("X-Kernel", _kernel.ToString());
            Response.Headers.Add("X-Target-Framework", _target_framework.ToString());
        }

        public void printLog()
        {
            _logger.LogInformation($"X-Total-Count: {_counter}");
            _logger.LogInformation($"X-Local: {_localhost}");
            _logger.LogInformation($"X-Kernel: {_kernel}");
            _logger.LogInformation($"X-Target-Framework: {_target_framework}");
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetEmps()
        {
            var header = Request.Headers["X-Debug"];

            if (header.ToString() == "true")
            {
                AddHttpHeaders(Response);
                printLog();
            }

            return await _context.Emps.ToListAsync();
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(long id)
        {
            var header = Request.Headers["X-Debug"];
            var cliente = await _context.Emps.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            if (header.ToString() == "true")
            {
                AddHttpHeaders(Response);
                printLog();
            }

            return cliente;
        }

        // PUT: api/clientes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(long id, Cliente cliente)
        {
            var header = Request.Headers["X-Debug"];

            if (id != cliente.Id)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (header.ToString() == "true")
            {
                AddHttpHeaders(Response);
                printLog();
            }

            return NoContent();
        }

        // POST: api/Clientes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            var header = Request.Headers["X-Debug"];

            _context.Emps.Add(cliente);
            await _context.SaveChangesAsync();

            if (header.ToString() == "true")
            {
                AddHttpHeaders(Response);
                printLog();
            }

            return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cliente>> DeleteCliente(long id)
        {
            var header = Request.Headers["X-Debug"];

            var cliente = await _context.Emps.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Emps.Remove(cliente);
            await _context.SaveChangesAsync();

            if (header.ToString() == "true")
            {
                AddHttpHeaders(Response);
                printLog();
            }

            return cliente;
        }

        private bool ClienteExists(long id)
        {
            return _context.Emps.Any(e => e.Id == id);
        }
    }
}
