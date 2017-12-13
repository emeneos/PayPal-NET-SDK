using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;

namespace PayPal.Vault.Test
{

    public class CreditCardUpdateTest : TestHarness
    {
        private List<JsonPatch> buildRequestBody()
        {
            var jsonContent = new StringContent("[ { \"op\": \"replace\", \"path\": \"/billing_address/line1\", \"value\": \"53 N Main St.\" }]", Encoding.UTF8, "application/json");
            return (List<JsonPatch>) new JsonSerializer().DeserializeResponse(jsonContent, typeof(List<JsonPatch>));
        }

        [Fact]
        public async void TestCreditCardUpdateRequest()
        {
            // Create
            HttpResponse createResponse = await CreditCardCreateTest.createCreditCard();
            var expected = createResponse.Result<CreditCard>();

            //  Update
            CreditCardUpdateRequest request = new CreditCardUpdateRequest(expected.Id);
            request.RequestBody(buildRequestBody());
                
            HttpResponse response = await client().Execute(request);
            Assert.Equal(200, (int) response.StatusCode);

            // Get
            HttpResponse getResponse = await CreditCardGetTest.getCreditCard(expected.Id);
            Assert.Equal(200, (int) getResponse.StatusCode);
            var updated = getResponse.Result<CreditCard>();
            Assert.NotNull(updated);
            Assert.Equal(updated.BillingAddress.Line1, "53 N Main St.");
        }
    }
}