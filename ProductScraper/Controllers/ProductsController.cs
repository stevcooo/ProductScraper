﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Exceptions;
using ProductScraper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductScraper.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductInfoService _productInfoService;
        private readonly IProductInfoHistoryService _productInfoHistoryService;

        public ProductsController(IProductInfoService productInfoService, IProductInfoHistoryService productInfoHistoryService)
        {
            _productInfoService = productInfoService;
            _productInfoHistoryService = productInfoHistoryService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _productInfoService.GetAllAsync(_userId));
        }

        // GET: Products
        public async Task<IActionResult> History(long id)
        {
            return PartialView(await _productInfoHistoryService.GetAllForProductAsync(id));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ProductInfo productInfo = await _productInfoService.GetDetailsAsync(_userId, id.Value);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        // GET: Products/Check
        public async Task<IActionResult> Check(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _productInfoService.CheckAsync(_userId, id.Value);
            }
            catch (Exception)
            {
                //Log error and notify user
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("URL")] ProductInfo productInfo)
        {
            if (ModelState.IsValid)
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                try
                {
                    await _productInfoService.AddAsync(_userId, productInfo);
                    try
                    {
                        await _productInfoService.CheckAsync(_userId, productInfo.Id);
                    }
                    catch (Exception)
                    {
                        //Log error and notify user
                    }

                }
                catch (ScrapeServiceException exception)
                {
                    ViewBag.Exception = exception.Message;
                    return View(productInfo);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productInfo);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ProductInfo productInfo = await _productInfoService.GetDetailsAsync(_userId, id.Value);
            if (productInfo == null)
            {
                return NotFound();
            }
            return View(productInfo);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,PartitionKey,RowKey,URL")] ProductInfo productInfo)
        {
            if (id != productInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                try
                {
                    await _productInfoService.UpdateAsync(_userId, productInfo);
                    await _productInfoService.CheckAsync(_userId, productInfo.Id);
                }
                catch (ScrapeServiceException exception)
                {
                    ViewBag.Exception = exception.Message;
                    return View(productInfo);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productInfo);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ProductInfo productInfo = await _productInfoService.GetDetailsAsync(_userId, id.Value);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            string _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _productInfoService.DeleteAsync(_userId, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
