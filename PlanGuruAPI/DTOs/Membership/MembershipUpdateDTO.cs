﻿namespace PlanGuruAPI.DTOs.Membership
{
    public class MembershipUpdateDTO
    {
        public Guid Id { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
