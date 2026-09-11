using CS.ERP_MOB.DB;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CS.ERP_MOB.Services.SYS
{
    public class Sys_Service
    {
        public Sys_Service()
        {
            //mApiConfig.APIProtocol = "https://";
        }
        public static ApiConfig mApiConfig = new ApiConfig();
        public static string mResponse = "";
        public async static Task<string> ApiCall(string argRequest, string argApiName)
        {
            try
            {
                StringContent content = new StringContent(argRequest, Encoding.UTF8, mApiConfig.ApiContentType);
                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(mAPIProtocol + mAPIServer + mAPIPort + mAPIServiceName);
                    client.BaseAddress = new Uri(mApiConfig.APIProtocol + mApiConfig.APIServer + mApiConfig.APIPort + mApiConfig.APIServiceName);
                    HttpResponseMessage response = await client.PostAsync(argApiName, content);

                    if (response.IsSuccessStatusCode)
                    {
                        mResponse = await response.Content.ReadAsStringAsync();
                        //Debug.WriteLine(responseData);
                    }
                    //else{}
                }
                return mResponse;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getUploadURL()
        {
            try
            {
                return mApiConfig.UploadURL;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getProductURL()
        {
            try
            {
                return mApiConfig.SwitchProductURL;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async static Task<string> UploadImageToServer(string uploadFolderName, string fieldName, string fileName, byte[] imageBytes)
        {
            try
            {
                using var client = new HttpClient();
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(uploadFolderName), "uploadfolderName");
                var imageContent = new ByteArrayContent(imageBytes);

                // Set appropriate content type
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                // Add folder name field
                content.Add(imageContent, fieldName, fileName);
                var url = mApiConfig.UploadURL + Sys_UploadFolder.sys_User;
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Assuming server returns { "fileName": "abc.jpg" }
                    var json = JsonDocument.Parse(responseBody);
                    if (json.RootElement.TryGetProperty("filename", out var fileNameElement))
                    {
                        return fileNameElement.GetString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
