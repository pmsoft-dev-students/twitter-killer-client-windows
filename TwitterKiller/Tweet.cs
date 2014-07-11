using System;

namespace TwitterKiller
{
    public class Tweet
    {
        public string Text { get; private set; }

        public DateTime Date { get; private set; }

        public long Id { get; private set; }

        public int UserId { get; private set; }

        public Tweet(string text, DateTime date, long id, int userId)
        {
            Text = text;
            Date = date;
            Id = id;
            UserId = userId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Tweet) obj);
        }

        protected bool Equals(Tweet other)
        {
            return string.Equals(Text, other.Text) && string.Equals(Date, other.Date) && Id == other.Id && UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Date.GetHashCode());
                hashCode = (hashCode*397) ^ Id.GetHashCode();
                hashCode = (hashCode*397) ^ UserId;
                return hashCode;
            }
        }
    }
}
