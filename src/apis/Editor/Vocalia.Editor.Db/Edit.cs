namespace Vocalia.Editor.Db
{
    public class Edit
    {
        public virtual int ID { get; set; }
        public virtual int ClipID { get; set; }
        public virtual int StartTrim { get; set; }
        public virtual int EndTrim { get; set; }

        public virtual Clip Clip { get; set; }
       
    }
}
