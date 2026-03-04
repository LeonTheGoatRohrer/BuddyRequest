namespace Models
{
    public class LocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class FriendLocationDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? LastLocationUpdate { get; set; }
        public string AvatarUrl { get; set; }
    }
}
