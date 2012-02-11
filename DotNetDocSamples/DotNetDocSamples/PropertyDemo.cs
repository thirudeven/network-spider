using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Emc.Documentum.FS.Doc.Samples.Client
{
    public class PropertyDemo
    {
        public Property[] showProperties()
        {
            Property[] properties =
            {
                new StringProperty("subject", "dangers"),
                new StringProperty("title", "Dangers"),
                new NumberProperty("short", (short) 1),
                new DateProperty("my_date", new DateTime()),
                new BooleanProperty("a_full_text", true),
                new ObjectIdProperty("my_object_id", new ObjectId("090007d280075180")),

                new StringArrayProperty("keywords", new String[]{"lions", "tigers", "bears"}),
                new NumberArrayProperty("my_number_array", (short) 1, 10, 100L, 10.10),
                new BooleanArrayProperty("my_boolean_array", true, false, true, false),
                new DateArrayProperty("my_date_array", new DateTime(), new DateTime()),
                new ObjectIdArrayProperty("my_obj_id_array",
                                          new ObjectId("0c0007d280000107"), new ObjectId("090007d280075180")),
            };
            return properties;
        }

        public void ShowNumberProperties()
        {
            PropertySet propertySet = new PropertySet();

            //Create instances of NumberProperty
            propertySet.Set("TestShortName", (short) 10);
            propertySet.Set("TestIntegerName", 10);
            propertySet.Set("TestLongName", 10L);
            propertySet.Set("TestDoubleName", 10.10);

            //Create instance of DateProperty
            propertySet.Set("TestDateName", new DateTime());

            //Create instance of BooleanProperty
            propertySet.Set("TestBooleanName", false);

            //Create instance of ObjectIdProperty
            propertySet.Set("TestObjectIdName", new ObjectId("10"));

            List<Property> properties = propertySet.Properties;
            foreach (Property p in properties)
            {
                Console.WriteLine(typeof(Property).ToString() +
                                  " = " +
                                  p.GetValueAsString());
            }
        }

        public void ShowTransient(ValidationInfoSet infoSet)
        {
            List<ValidationInfo> failedItems = infoSet.ValidationInfos;
            foreach (ValidationInfo vInfo in failedItems)
            {
                Console.WriteLine(vInfo.DataObject.Properties.Get("my_unique_id"));
            }
        }

        public PropertySet ShowPropertySet()
        {
            Property[] properties =
                {
                    new StringProperty("subject", "dangers"),
                    new StringProperty("title", "Dangers"),
                    new StringArrayProperty("keywords", new String[]{"lions", "tigers", "bears"}),
                };
            PropertySet propertySet = new PropertySet();
            foreach (Property property in properties)
            {
                propertySet.Set(property);
            }
            return propertySet;
        }
    }
}
