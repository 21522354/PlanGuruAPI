﻿using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PlanGuruAPI.DTOs.ChatDTOs;
using PlanGuruAPI.Hubs;
using System.Linq.Expressions;

namespace PlanGuruAPI.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly PlanGuruDBContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IMapper mapper, PlanGuruDBContext context, IHubContext<ChatHub> hubContext)
        {
            _mapper = mapper;
            _context = context;
            _hubContext = hubContext;
        }
        [HttpGet("users/{userId}/chatRooms")]
        public async Task<IActionResult> GetAllChatRoomForUser(Guid userId)
        {
            var chatRooms = await _context.ChatRooms.Where(
                p => p.FirstUserId == userId || p.SecondUserId == userId).ToListAsync();
            var listChatRoomReadDTO = new List<ChatRoomReadDTO>();
            foreach (var chatRoom in chatRooms)
            {
                var userID = userId == chatRoom.FirstUserId ? chatRoom.SecondUserId : chatRoom.FirstUserId;
                var friend = await _context.Users.FindAsync(userID);
                if (friend == null) {
                    return BadRequest("Can't find this user");
                }
                var lastMessage = await _context.ChatMessages.Where(p => p.ChatRoomId == chatRoom.ChatRoomId).OrderByDescending(p => p.SendDate).FirstOrDefaultAsync();
                var chatRoomReadDTO = new ChatRoomReadDTO()
                {
                    ChatRoomId = chatRoom.ChatRoomId,
                    UserId = friend.UserId,
                    Avatar = friend.Avatar,
                    Name = friend.Name,
                    IsOnline = false,
                };
                if(lastMessage != null)
                {
                    chatRoomReadDTO.LastMessage = lastMessage.Message;
                    chatRoomReadDTO.LastMessageTime = lastMessage.SendDate;
                }
                listChatRoomReadDTO.Add(chatRoomReadDTO);
            }
            listChatRoomReadDTO = listChatRoomReadDTO.OrderByDescending(p => p.LastMessageTime).ToList();
            return Ok(listChatRoomReadDTO);
        }
        [HttpGet("{chatRoomId}")]
        public async Task<IActionResult> GetCertainChat(Guid chatRoomId)
        {
            var listChatMessage = await _context.ChatMessages.Where(p => p.ChatRoomId == chatRoomId).ToListAsync();
            var listChatMessageReadDTO = new List<ChatMessageReadDTO>();
            var chatRoom = await _context.ChatRooms.FindAsync(chatRoomId);
            if(chatRoom == null)
            {
                return BadRequest("Can't found this ChatRoom");
            }
            var firstUser = await _context.Users.FindAsync(chatRoom.FirstUserId);
            var secondUser = await _context.Users.FindAsync(chatRoom.SecondUserId);
            foreach (var chatMessage in listChatMessage)
            {
                var user = chatMessage.UserSendId == firstUser.UserId ? firstUser : secondUser;
                var chatMessageReadDTO = new ChatMessageReadDTO()
                {
                    UserSendId = chatMessage.UserSendId,
                    Avatar = user.Avatar,
                    ChatMessageId = chatMessage.ChatMessageId,
                    MediaLink = chatMessage.MediaLink,
                    Message = chatMessage.Message,
                    SendDate = chatMessage.SendDate,
                    Type = chatMessage.Type,
                };
                listChatMessageReadDTO.Add(chatMessageReadDTO);
            }
            return Ok(listChatMessageReadDTO);
        }
        [HttpPost("sendText")]
        public async Task<IActionResult> SendText(SendTextRequest request)
        {
            var firstUser = await _context.Users.FindAsync(request.UserSendId);
            var secondUser = await _context.Users.FindAsync(request.UserReceiveId);
            if (firstUser == null || secondUser == null)
            {
                return BadRequest("Can't find this user");
            }
            var chatRoom = await _context.ChatRooms.Where(
                p => p.FirstUserId == request.UserSendId && p.SecondUserId == request.UserReceiveId ||
                    p.FirstUserId == request.UserReceiveId && p.SecondUserId == request.UserSendId).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                chatRoom = new ChatRoom()
                {
                    ChatRoomId = Guid.NewGuid(),
                    FirstUserId = request.UserSendId,
                    SecondUserId = request.UserReceiveId,
                };
                await _context.ChatRooms.AddAsync(chatRoom);
            }
            var chatMessage = new ChatMessage()
            {
                ChatMessageId = Guid.NewGuid(),
                ChatRoomId = chatRoom.ChatRoomId,
                MediaLink = null,
                Message = request.Message,
                UserSendId = request.UserSendId,
                SendDate = DateTime.UtcNow,
                Type = "Text"
            };
            await _context.ChatMessages.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("NewMessage", secondUser.Id.ToString());
            return Ok("Send text successfully");
        }
        [HttpPost("sendMedia")]
        public async Task<IActionResult> SendMedia(SendMediaRequest request)
        {
            var firstUser = await _context.Users.FindAsync(request.UserSendId);
            var secondUser = await _context.Users.FindAsync(request.UserReceiveId);
            if (firstUser == null || secondUser == null)
            {
                return BadRequest("Can't find this user");
            }
            var chatRoom = await _context.ChatRooms.Where(
                p => p.FirstUserId == request.UserSendId && p.SecondUserId == request.UserReceiveId ||
                    p.FirstUserId == request.UserReceiveId && p.SecondUserId == request.UserSendId).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                chatRoom = new ChatRoom()
                {
                    ChatRoomId = Guid.NewGuid(),
                    FirstUserId = request.UserSendId,
                    SecondUserId = request.UserReceiveId,
                };
                await _context.ChatRooms.AddAsync(chatRoom);
            }
            foreach(var img in request.Images)
            {
                var chatMessage = new ChatMessage()
                {
                    ChatMessageId = Guid.NewGuid(),
                    ChatRoomId = chatRoom.ChatRoomId,
                    MediaLink = img,
                    Message = null,
                    UserSendId = request.UserSendId,
                    SendDate = DateTime.UtcNow,
                    Type = "Media"
                };
                await _context.ChatMessages.AddAsync(chatMessage);  
            }
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("NewMessage", secondUser.Id.ToString());
            return Ok("Send media successfully");
        }
    }
}
