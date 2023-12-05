using coffee_shop_backend.Business.Abstracts;
using coffee_shop_backend.Contexs;
using coffee_shop_backend.Entitys.Concreates;
using Nest;

namespace coffee_shop_backend.Business.Concreates;

public class ProductElasticSearchManager<T>: IProductElasticSearchServices<T> where T : class
{

    private readonly ElasticClient _client;
    private readonly CoffeeShopDbContex _coffeeShopDbContex;


    public ProductElasticSearchManager(ElasticClient client, CoffeeShopDbContex coffeeShopDbContex)
    {
        var settings = new ConnectionSettings( new Uri("http://localhost:9200"))
            .DefaultIndex("coffee-shop");
        _client = new ElasticClient(settings);
        _coffeeShopDbContex = coffeeShopDbContex;
    }

    public ISearchResponse<T> Search(string query, string token)
    {

        var result = _client.Search<Product>(s => s
            .Query(q => q
                .MatchPhrase(m => m
                    .Field(f => f.Name) // 'Name' özelliğine doğrudan erişim
                    .Query(query)
                )
            )
        );

        return (ISearchResponse<T>)result;
    }

    public void IndexDataFromDatabase()
    {
        var products = _coffeeShopDbContex.Products.ToList();
        var response = _client.IndexMany(products);
    }
}