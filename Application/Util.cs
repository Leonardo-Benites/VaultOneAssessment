//using Assessment.Application.Dtos;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore.Query.Internal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.NetworkInformation;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application
//{
//    public static class Util
//    {
//        public static async Task<byte[]> ConvertFileToByteAsync(IFormFile? file)
//        {
//            if (file != null)
//            {
//                using var memoryStream = new MemoryStream();
//                await file.CopyToAsync(memoryStream);
//                return memoryStream.ToArray();
//            }

//            return Array.Empty<byte>();
//        }
//    }
//}
