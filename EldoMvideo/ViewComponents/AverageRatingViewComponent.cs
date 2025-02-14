using EldoMvideo.Models;
using EldoMvideoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace EldoMvideo.ViewComponents;

public class AverageRatingViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int product_id)
    {
        List<Review> reviews = await ApiHelper.Get<List<Review>>("reviews");
        double averageRating = reviews
            .Where(r => r.product_id == product_id)
            .Select(r => r.rating)
            .DefaultIfEmpty(0)
            .Average(); 
        
        
        return View(averageRating);
    }
}