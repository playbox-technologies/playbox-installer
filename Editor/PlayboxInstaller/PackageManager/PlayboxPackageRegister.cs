#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using PlayboxInstaller;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Editor.PlayboxInstaller.PackageManager
{
    public static class PlayboxPackageRegister
    {
        private static List<GitDependentiesLink> _dependentiesLinks = new();
        
        private const string PLAYBOX_KEY = "PLAYBOX_KEY";

        private static DateTime _lastUpdate;
        private const int _updateRateInMinutes = 5;

        public static List<GitDependentiesLink> DependentiesLinks
        {
            get => _dependentiesLinks;
        }

        public static int UpdateRateInMinutes => _updateRateInMinutes;
        
        public static DateTime LastUpdate => _lastUpdate;
        
        private static bool _isFirstRun = true;

        public static void Initialize()
        {
            _lastUpdate = DateTime.UtcNow;
        }

        public static async void Register()
        {
            if (!_isFirstRun)
            {
                if ((DateTime.UtcNow - _lastUpdate).TotalMinutes <= _updateRateInMinutes)
                    return;
            }
            else
            {
                _isFirstRun = false;
            }

            _dependentiesLinks.Clear();

                string repositories;
                
                var time  = DateTime.UtcNow;
                
                if (PlayboxMemoryCache.Exists(PLAYBOX_KEY))
                {
                    repositories = (string)PlayboxMemoryCache.Get(PLAYBOX_KEY).Element;
                }
                else
                {
                    var result = await HttpHelper.GetAsync("https://api.github.com/orgs/playbox-technologies/repos?type=public");  
                    
                    repositories = result.Body;
                    
                    PlayboxMemoryCache.Push(
                        new KeyValuePair<string, PlayboxCacheElement>(PLAYBOX_KEY, new PlayboxCacheElement(repositories, UpdateRateInMinutes)));

                    _lastUpdate = PlayboxMemoryCache.Get(PLAYBOX_KEY).LastUpdate;
                }
                
                Debug.Log($"Repositories update time: {(DateTime.UtcNow - time).TotalMilliseconds} milliseconds)");
                
                if (string.IsNullOrEmpty(repositories))
                    return;
            
                JArray repositoriesArray = JArray.Parse(repositories);

                var time2 = DateTime.UtcNow;
                
                foreach (var repository in repositoriesArray)
                {
                    GitDependentiesLink dependentiesLink = new();
                
                    dependentiesLink.gitOrganization = repository["owner"]?["login"]?.Value<string>();
                    dependentiesLink.gitProjectName = repository["name"]?.Value<string>();
                    dependentiesLink.gitDefaultBranch = repository["default_branch"]?.Value<string>();
                    dependentiesLink.isHashedLock = false;

                    string branchesURL = dependentiesLink.GetBranchesURL();

                    string brachBodyString;

                    string projectName = dependentiesLink.gitProjectName;
                    
                    if (PlayboxMemoryCache.Exists(projectName))
                    {
                        brachBodyString = (string)PlayboxMemoryCache.Get(projectName).Element;
                    }
                    else
                    {
                        var result = await HttpHelper.GetAsync(branchesURL);  
                    
                        brachBodyString = result.Body;
                    
                        PlayboxMemoryCache.Push(
                            new KeyValuePair<string, PlayboxCacheElement>(projectName, new PlayboxCacheElement(brachBodyString, 2)));
                        
                    }

                    var branchesBody = JArray.Parse(brachBodyString);

                    foreach (var branch in branchesBody)
                    {
                        string branchName = branch["name"]?.Value<string>();
                    
                        dependentiesLink.RegisterBranch(branchName);
                    }
                
                    dependentiesLink.ResetToDefaultBranch();
                
                    if(!_dependentiesLinks.Contains(dependentiesLink))
                        _dependentiesLinks.Add(dependentiesLink);
                }
                
                Debug.Log($"Branches update time: {(DateTime.UtcNow - time2).TotalMilliseconds} milliseconds");
        }

        public static void UpdateNow()
        {
            PlayboxMemoryCache.Clear();
            Register();
        }
    }
}

#endif