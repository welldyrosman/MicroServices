﻿using System;
using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps() {
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<ProductDto, Product>().ReverseMap();
            });
			return mappingConfig;
		}
	}
}


