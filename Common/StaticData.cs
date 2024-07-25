namespace EmployeMasterCrud.Common
{
    public class StaticData
    {
        public static List<KeyValueModel> UserTypeDropdown()
        {
            List<KeyValueModel> list = new List<KeyValueModel>();
            list.Add(new KeyValueModel
            {
                name = "Administrator",
                value = "1"
            });

            list.Add(new KeyValueModel
            {
                name = "User",
                value = "2"
            });

            return list;
        }

        public static List<boolValueModel> IsPublishDD()
        {
            List<boolValueModel> list = new List<boolValueModel>();

            list.Add(new boolValueModel
            {
                name = "Yes",
                value = true
            });

            list.Add(new boolValueModel
            {
                name = "No",
                value = false
            });

            return list;
        }

        public static List<intValueModel> MediaTypeDD()
        {
            List<intValueModel> list = new List<intValueModel>();

            list.Add(new intValueModel
            {
                name = "Image",
                value = 1
            });

            list.Add(new intValueModel
            {
                name = "Video",
                value = 2
            });

            return list;
        }

        public static List<intValueModel> genderDD()
        {
            List<intValueModel> list = new List<intValueModel>();

            list.Add(new intValueModel
            {
                name = "Male",
                value = 1
            });

            list.Add(new intValueModel
            {
                name = "Female",
                value = 2
            });

            list.Add(new intValueModel
            {
                name = "Other",
                value = 3
            });

            return list;
        }
    }

    public class KeyValueModel
    {
        public string name { get; set; } = null!;
        public string value { get; set; } = null!;
    }

    public class boolValueModel
    {
        public string name { get; set; } = null!;

        public bool value { get; set; }
    }

    public class intValueModel
    {
        public string name { get; set; } = null!;

        public int value { get; set; }
    }

    public class stateValueModel
    {
        public string StateName { get; set; } = null!;

        public int StateId { get; set; }
    }

    public class cityValueModel
    {
        public string CityName { get; set; } = null!;

        public int CityId { get; set; }
    }
}
