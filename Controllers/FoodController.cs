using Amazon.S3;
using Amazon.S3.Transfer;
using FoodDelivery4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace FoodDelivery4.Controllers
{
    public class FoodController : Controller
    {
        private readonly string accessKey = "";  
        private readonly string secretKey = "";  
        private readonly string bucketName = "bcaballero";  
        private readonly string region = "us-east-1";  

        // AWS S3 client initialization
        private AmazonS3Client CreateS3Client()
        {
            return new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.USEast1);
        }

        // Helper method to upload image to S3 and return the URL
        private async Task<string> UploadImageToS3(IFormFile file)
        {
            var client = CreateS3Client();
            var fileTransferUtility = new TransferUtility(client);

            string fileUrl = string.Empty;
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var key = Guid.NewGuid().ToString() + Path.GetExtension(fileName); 

                using (var stream = file.OpenReadStream())
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = key,
                        InputStream = stream,
                        ContentType = file.ContentType,
                        CannedACL = S3CannedACL.PublicRead 
                    };
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                // Generate a URL to access the image on S3
                fileUrl = $"https://{bucketName}.s3.{region}.amazonaws.com/{key}";
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error encountered while uploading image: {e.Message}");
            }

            return fileUrl;
        }

        // GET: FoodController
        public async Task<ActionResult> Index()
        {
            string apiUrl = "https://localhost:7296/api/food";  

            List<food> foods = new List<food>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                var result = await response.Content.ReadAsStringAsync();
                foods = JsonConvert.DeserializeObject<List<food>>(result);
            }

            return View(foods);
        }

        // GET: FoodController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FoodController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(food food, IFormFile file)
        {
            if (file != null)
            {
                // Basic file validation
                if (file.Length > 5 * 1024 * 1024)  
                {
                    ModelState.AddModelError(string.Empty, "File size cannot exceed 5MB.");
                    return View(food);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(string.Empty, "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                    return View(food);
                }

                // Upload the image to S3 and get the image URL
                try
                {
                    food.ImageUrl = await UploadImageToS3(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Image upload failed: {e.Message}");
                    ModelState.AddModelError(string.Empty, "Image upload failed. Please try again.");
                    return View(food);
                }
            }

            // Save food data via API call
            string apiUrl = "https://localhost:7296/api/food";  
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(food), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));  
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the food item.");
                }
            }

            return View(food);
        }
    }
}

