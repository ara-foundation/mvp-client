// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Collections.Generic;
using System;
using UnityEngine;

public class Attributes
{
    public string title { get; set; }
    public string slug { get; set; }
    public int commentCount { get; set; }
    public int participantCount { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime lastPostedAt { get; set; }
    public int lastPostNumber { get; set; }
    public string username { get; set; }
    public string displayName { get; set; }
    public string avatarUrl { get; set; }
    public int number { get; set; }
    public string contentType { get; set; }
    public string contentHtml { get; set; }
    public bool renderFailed { get; set; }
    public int mentionedByCount { get; set; }
}

public class AraIdeaDatum
{
    public string type { get; set; }
    public string id { get; set; }
    public Attributes attributes { get; set; }
    public Relationships relationships { get; set; }
}

public class FirstPost
{
    public string type { get; set; }
    public string id { get; set; }
    public Attributes attributes { get; set; }
}

public class Links
{
    public string first { get; set; }
    public string? next { get; set; }
    public string? prev { get; set; }
}

public class Relationships
{
    public AraUser user { get; set; }
    public FirstPost firstPost { get; set; }
}

public class AraIdeas
{
    public Links links { get; set; }
    public List<AraDiscussion> data { get; set; }
}

public class AraUser
{
    public int id { get; set; }
    public string token { get; set; }
    public AraUserAttributes attributes { get; set; }
}

public class AraUserAttributes
{
    public string username { get; set; }
    public string displayName { get; set; }
    public string avatarUrl { get; set; }
    public string slug { get; set; }
    public int? discussionCount { get; set; }
    public int? commentCount { get; set; }
    public string lastSeenAt { get; set; }
    public bool? isEmailConfirmed { get; set; }
    public string email { get; set; }
    public int? points { get; set; }
}

public class AraDiscussion
{
    public string type { get; set; }
    public int id { get; set; }
    public AraDiscussionAttributes attributes { get; set; }
    public AraDiscussionRelationships relationships { get; set; }
}

public class AraDiscussionAttributes
{
    public string title { get; set; }
    public string slug { get; set; }
    public int? commentCount { get; set; }
    public int? participantCount { get; set; }
    public string createdAt { get; set; }
    public string lastPostedAt { get; set; }
    public int? lastPostNumber { get; set; }
}

public class RelationshipElement
{
    public string type { get; set; }
    public object id { get; set; }
}

public class IncludedUser : RelationshipElement
{
    public UserAttributes attributes { get; set; }
}

public class UserAttributes
{
    public string username { get; set; }
    public string displayName { get; set; }
    public string avatarUrl { get; set; }
    public string slug { get; set; }
}

public class IncludedPost : RelationshipElement
{
    public PostAttributes attributes { get; set; }
}

public class PostAttributes
{
    public int number { get; set; }
    public string createdAt { get; set; }
    public string contentType { get; set; }
    public string contentHtml { get; set; }
    public bool renderFailed { get; set; }
    public int mentionedByCount { get; set; }
}

public class AraDiscussionRelationships
{
    public IncludedUser user { get; set; }
    public IncludedPost firstPost { get; set; }
}

