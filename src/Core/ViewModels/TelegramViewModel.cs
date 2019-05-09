namespace Core.ViewModels
{
    public abstract class SocialNetworkViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        
        public string Link { get; set; }
    }
    
    public class TelegramViewModel : SocialNetworkViewModel
    {
    }
    
    public class FacebookViewModel : SocialNetworkViewModel
    {
    }
    
    public class TwitterViewModel : SocialNetworkViewModel
    {
    }
}