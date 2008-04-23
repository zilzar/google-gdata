/* Copyright (c) 2006 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
#define USE_TRACING
#define DEBUG

using System;
using System.IO;
using System.Xml; 
using System.Collections;
using System.Configuration;
using System.Net; 
using NUnit.Framework;
using Google.GData.Client;
using Google.GData.Client.UnitTests;
using Google.GData.Extensions;
using Google.GData.Contacts;





namespace Google.GData.Client.LiveTests
{
    [TestFixture] 
    [Category("LiveTest")]
    public class ContactsTestSuite : BaseLiveTestClass
    {
      //////////////////////////////////////////////////////////////////////
        /// <summary>default empty constructor</summary> 
        //////////////////////////////////////////////////////////////////////
        public ContactsTestSuite()
        {
        }

        public override string ServiceName
        {
            get {
                return ContactsService.GContactService;
            }
        }



        //////////////////////////////////////////////////////////////////////
        /// <summary>runs an authentication test</summary> 
        //////////////////////////////////////////////////////////////////////
        [Test] public void ContactsAuthenticationTest()
        {
            Tracing.TraceMsg("Entering ContactsAuthenticationTest");

            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri(this.userName + "@googlemail.com"));
            ContactsService service = new ContactsService("unittests");

            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }

            ContactsFeed feed = service.Query(query);

            ObjectModelHelper.DumpAtomObject(feed,CreateDumpFileName("ContactsAuthTest")); 

            if (feed != null && feed.Entries.Count > 0)
            {
                Tracing.TraceMsg("Found a Feed " + feed.ToString());

                foreach (ContactEntry entry in feed.Entries)
                {
                    Tracing.TraceMsg("Found an entry " + entry.ToString());
                }
            }
        }
        /////////////////////////////////////////////////////////////////////////////


        
        //////////////////////////////////////////////////////////////////////
        /// <summary>runs an authentication test, inserts a new contact</summary> 
        //////////////////////////////////////////////////////////////////////
        [Test] public void InsertContactsTest()
        {
            Tracing.TraceMsg("Entering InsertContactsTest");

            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri(this.userName + "@googlemail.com")); 
            ContactsService service = new ContactsService("unittests");

            if (this.userName != null)
            {
                service.Credentials = new GDataCredentials(this.userName, this.passWord);
            }

            ContactsFeed feed = service.Query(query);

            ArrayList inserted = new ArrayList();
            if (feed != null)
            {
                Assert.IsTrue(feed.Entries != null, "the contacts needs entries");

                for (int i=0; i<3; i++)
                {
                    ContactEntry entry = ObjectModelHelper.CreateContactEntry(i);
                    inserted.Add(feed.Insert(entry));
                }
            }

            if (inserted.Count > 0)
            {
                int iVer = inserted.Count;
                feed = service.Query(query);

                // let's find those guys
                for (int i = 0; i < inserted.Count; i++)
                {
                    ContactEntry test = inserted[i] as ContactEntry;
                    foreach (ContactEntry e in feed.Entries)
                    {
                        if (e.Id == test.Id)
                        {
                            iVer--;
                        }
                    }
                }
                Assert.IsTrue(iVer == 0, "The new entries should all be part of the feed now");
            }

            int all = feed.Entries.Count;


            // now delete them again

            foreach (ContactEntry e in inserted)
            {
                e.Delete();
            }

            // now make sure they are gone
            if (inserted.Count > 0)
            {
                int iVer = inserted.Count;
                feed = service.Query(query);

                // let's find those guys
                for (int i = 0; i < inserted.Count; i++)
                {
                    ContactEntry test = inserted[i] as ContactEntry;
                    foreach (ContactEntry e in feed.Entries)
                    {
                        if (e.Id == test.Id)
                        {
                            iVer--;
                        }
                    }
                }
                Assert.IsTrue(iVer == 3, "The new entries should all be deleted now");
                Assert.IsTrue(feed.Entries.Count == all - 3, "The count should be correct as well");
            }
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>Tests the primary Accessors</summary> 
        //////////////////////////////////////////////////////////////////////
        [Test]
        public void TestPrimaryContactsProperties()
        {
            Tracing.TraceMsg("Entering TestPrimaryContactsProperties");

            ContactEntry entry = new ContactEntry();

            EMail e = new EMail();
            e.Primary = true;
            e.Address = "joe@smith.com";

            Assert.IsTrue(entry.PrimaryEmail == null, "Entry should have no primary Email");
            entry.Emails.Add(e);
            Assert.IsTrue(entry.PrimaryEmail == e, "Entry should have one primary Email");

            entry.Emails.Remove(e);
            Assert.IsTrue(entry.PrimaryEmail == null, "Entry should have no primary Email");

            PostalAddress p = new PostalAddress("Testaddress");
            p.Primary = true;
            Assert.IsTrue(entry.PrimaryPostalAddress == null, "Entry should have no primary Postal");
            entry.PostalAddresses.Add(p);
            Assert.IsTrue(entry.PrimaryPostalAddress == p, "Entry should have one primary Postal");
            entry.PostalAddresses.Remove(p);
            Assert.IsTrue(entry.PrimaryPostalAddress == null, "Entry should have no primary Postal");

            PhoneNumber n = new PhoneNumber("123345");
            n.Primary = true;

            Assert.IsTrue(entry.PrimaryPhonenumber == null, "Entry should have no primary Phonenumber");
            entry.Phonenumbers.Add(n);
            Assert.IsTrue(entry.PrimaryPhonenumber == n, "Entry should have one primary Phonenumber");

            entry.Phonenumbers.Remove(n);
            Assert.IsTrue(entry.PrimaryPhonenumber == null, "Entry should have no primary Phonenumber");

            IMAddress i = new IMAddress("joe@smight.com");
            i.Primary = true;

            Assert.IsTrue(entry.PrimaryIMAddress == null, "Entry should have no primary IM");
            entry.IMs.Add(new IMAddress());
            entry.IMs.Add(i);
            Assert.IsTrue(entry.PrimaryIMAddress == i, "Entry should have one primary IMAddress");

            entry.IMs.Remove(i);
            Assert.IsTrue(entry.PrimaryIMAddress == null, "Entry should have no primary IM");
        }

    }
}



