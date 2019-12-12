/*
Function: 		Provide generic map to load and store game content AND allow dispose() to be called on all content
Author: 		NMCG
Version:		1.0
Date Updated:	11/9/17
Bugs:			None
Fixes:			None
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace GDLibrary
{
    public class ContentDictionary<V> : IDisposable
    {
        #region Fields
        private string name;
        private Dictionary<string, V> dictionary;
        private ContentManager content;
        #endregion

        #region Properties
        protected Dictionary<string, V> Dictionary
        {
            get
            {
                return dictionary;
            }
        }

        public V this[string key]
        {
            get
            {
                if (!this.Dictionary.ContainsKey(key))
                    throw new Exception(key + " resource was not found in dictionary. Have you loaded it?");

                return this.dictionary[key];
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        #endregion

        #region Constructors
        public ContentDictionary(
            string name,
            ContentManager content
        )
        {
            this.name = name;
            this.content = content;
            this.dictionary = new Dictionary<string, V>();
        }
        #endregion

        #region Methods
        //Add to dictionary using key
        public virtual bool Load(string assetPath, string key)
        {
            if (!this.dictionary.ContainsKey(key))
            {
                this.dictionary.Add(key, this.content.Load<V>(assetPath));
                return true;
            }
            return false;
        }

        //Add to dictionary without key
        public virtual bool Load(string assetPath)
        {
            return Load(assetPath, StringUtility.ParseNameFromPath(assetPath));
        }

        //Remove from dictionary
        public virtual bool Unload(string key)
        {
            if (this.dictionary.ContainsKey(key))
            {
                //Unload from RAM
                Dispose(this.dictionary[key]);

                //Remove from dictionary
                this.dictionary.Remove(key);
                return true;
            }
            return false;
        }

        //Returns count of the pairs in dictionary
        public virtual int Count()
        {
            return this.dictionary.Count;
        }

        //Free object from RAM - Generic
        public virtual void Dispose()
        {
            //Copy values from dictionary to list
            List<V> list = new List<V>(dictionary.Values);

            //Dispose each item in the list
            for (int i = 0; i < list.Count; i++)
                Dispose(list[i]);

            //Empty list
            list.Clear();

            //Clear dictionary
            this.dictionary.Clear();
        }

        //Free object from RAM
        public virtual void Dispose(V value)
        {
            //If a disposable object (e.g. model, sound, font, texture)
            if (value is IDisposable)

                //Call object dispose method
                ((IDisposable)value).Dispose();

            //If a user-defined or C# object
            else

                //Set to null for garbage collection
                value = default(V);
        }
        #endregion
    }
}