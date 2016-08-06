using AlchemyAPIClient;
using AlchemyAPIClient.Requests;
using AlchemyAPIClient.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JobminxFinal.Helpers
{

    class resume
    {
        public List<Dictionary<string, string>> Experience { get; set; }
        public List<string> Education { get; set; }
        public List<string> Competencies { get; set; }
        public Dictionary<string, string> SingleValues { get; set; }
        public string ResumeText { get; set; }
        public List<string> Taxonomies { get; set; }
        public string BestTax { get; set; }
    }

    class alchemyCompare
    {
        public List<string> Competencies { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Concepts { get; set; }
        public List<string> Skills { get; set; }
        public string JobTitle { get; set; }
        public string Text { get; set; }
    }

    class alchemy
    {
        public async Task<AlchemyKeywordsResponse> GetKeywords(string text, AlchemyClient client)
        {
            var request = new AlchemyTextKeywordsRequest(text, client)
            {
                KnowledgeGraph = true,
                ShowSourceText = true,
                Sentiment = true,
                KeyWordExtractModeText = KeyWordExtractModeType.Strict
            };
            return await request.GetResponse();
        }

        public async Task<AlchemyEntitiesResponse> GetEntities(string text, AlchemyClient client)
        {
            var request = new AlchemyTextEntitiesRequest(text, client)
            {
                Coreference = true,
                Disambiguate = true,
                KnowledgeGraph = true,
                LinkedData = true,
                Quotations = true,
                Sentiment = true,
                ShowSourceText = true,
                StructuredEntities = true
            };
            return await request.GetResponse();
        }

        public async Task<AlchemyConceptsResponse> GetConcepts(string text, AlchemyClient client)
        {
            var request = new AlchemyTextConceptsRequest(text, client)
            {
                KnowledgeGraph = true,
                LinkedData = true,
                ShowSourceText = true
            };
            return await request.GetResponse();
        }

        public async Task<AlchemyTaxonomiesResponse> GetTaxonomies(string text, AlchemyClient client)
        {
            var request = new AlchemyTextTaxonomiesRequest(text, client)
            {
            };
            return await request.GetResponse();
        }

        public async Task<AlchemyTextResponse> GetCleanText(Uri url, AlchemyClient client)
        {
            var request = new AlchemyUrlCleanTextRequest(url, client)
            {
                ExtractLinks = true,
                UseMetadata = true

            };
            return await request.GetResponse();
        }
    }
}