using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Models
{
    public class User : INotifyPropertyChanged
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public string? Key { get; set; }

        // Avatar & Profil
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Avatar-URL als Property (nicht Methode!)
        [NotMapped]
        public string Avatar
        {
            get
            {
                if (!string.IsNullOrEmpty(AvatarUrl))
                    return AvatarUrl;
                
                // Fallback: Automatisch generierter Avatar
                return $"https://api.dicebear.com/7.x/avataaars/png?seed={Username}";
            }
        }

        private bool _requestSent = false;
        
        // UI-Status für Freundschaftsanfrage (nicht in DB)
        [NotMapped]
        public bool RequestSent 
        { 
            get => _requestSent;
            set
            {
                if (_requestSent != value)
                {
                    _requestSent = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(RequestButtonText));
                }
            }
        }

        [NotMapped]
        public string RequestButtonText => RequestSent ? "Anfrage bereits gesendet" : "Anfrage senden";

        // Alte Methode für Backward-Compatibility
        public string GetAvatarUrl()
        {
            return Avatar;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
