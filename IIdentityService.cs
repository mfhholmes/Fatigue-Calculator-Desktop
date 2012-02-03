using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatigue_Calculator_Desktop
{
 
    public interface IIdentityService
    {
        /// <summary>
        /// sets the source URL for the identity source. Depends on implementation, but usually filename or web service URL
        /// </summary>
        /// <param name="sourceURL">the source URL for the service specifed</param>
        /// <returns>true if the source is valid, false if not</returns>
        bool SetIdentityListSource(string sourceURL);

        /// <summary>
        /// Looks up the given id reference in the identity source and returns all matches containing the lookup. e.g. looking up "a1" returns "a111", "2a1", "aba121"
        /// </summary>
        /// <param name="Id">the Id reference to look up</param>
        /// <returns>list of identity objects with all matches</returns>
        List<identity> LookUpId(string Id);

        /// <summary>
        /// Looks up the given name in the identity source and returns all matches contianing the lookup. e.g. looking up 'dave' returns "Dave Smith", "Richard Davenport", "Mike Gondaven"
        /// </summary>
        /// <param name="Name">the name to look up</param>
        /// <returns>list of identity objects with all matches</returns>
        List<identity> LookUpName(string Name);

        /// <summary>
        /// Finds any exact matches in the identity source for the Id passed in. e.g "a1" will match "A1" but not "a11", "2a1" or "11aa11"
        /// </summary>
        /// <param name="Id">the Id to find</param>
        /// <returns>the list of exact matches to the searched Id</returns>
        List<identity> ExactMatchId(string Id);

        /// <summary>
        /// Finds any exact matches in the identity source for the name passed in. e.g. "dave" will match "Dave" but not "Dave Smith"
        /// </summary>
        /// <param name="Name">the name to search for</param>
        /// <returns>any identities found in the identity source that have the name searched for</returns>
        List<identity> ExactMatchName(string Name);

        /// <summary>
        /// adds a new identity to the storage method used by the service
        /// </summary>
        /// <param name="newId">the new identity to add to the store. May not necessarily be unique, up to the service to determine how to handle duplicates</param>
        /// <returns>true if the identity was successfully added</returns>
        bool AddNewIdentity(identity newId);

        /// <summary>
        /// returns the entire list of identities
        /// </summary>
        /// <param name="noduplicates">true (default) if the list should be pruned of duplicates</param>
        /// <returns>List of identity objects</returns>
        List<identity> IdentityList(bool noduplicates = true);
    }
}
