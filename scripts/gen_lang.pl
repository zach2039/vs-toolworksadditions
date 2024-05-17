#!/bin/perl

use strict;
use warnings;

my %woodtypes = (
	mahogany => 'Mahogany',
	douglasfir => 'Douglas fir',
	honeylocust => 'Honey locust',
	elm => 'Elm',
	beech => 'Beech',
	bearnut => 'Bearnut',
	eucalyptus => 'Eucalyptus',
	catalpa => 'Catalpa',
	cedar => 'Cedar',
	sal => 'Sal',
	saxaul => 'Saxaul',
	spruce => 'Spruce',
	sycamore => 'Sycamore',
	walnut => 'Walnut',
	willow => 'Willow',
	tuja => 'Tuja',
	redcedar => 'Red cedar', 
	yew => 'Yew', 
	kauri => 'Kauri', 
	ginkgo => 'Ginko', 
	dalbergia => 'Dalbergia', 
	umnini => 'Umnini', 
	banyan => 'Banyan', 
	poplar => 'Poplar', 
	guajacum => 'Gaujacum', 
	ghostgum => 'Ghost gum', 
	ohia => 'Ohia', 
	satinash => 'Satinash', 
	bluemahoe => 'Blue mahoe', 
	jacaranda => 'Jacaranda', 
	empresstree => 'Empress tree', 
	chlorociboria => 'Chlorociboria', 
	petrified => 'Petrified'
);

printf "{\n";

foreach my $woodtype (keys %woodtypes) {
	my $en = $woodtypes{$woodtype};
	my $enl = lc($en);
	printf "\t\"toolworks:item-handle-${woodtype}-none-dry\": \"Untreated ${enl} handle\",\n";
	printf "\t\"toolworks:item-handle-${woodtype}-*-wet\": \"Unset ${enl} handle\",\n";
	printf "\t\"toolworks:item-handle-${woodtype}-*-dry\": \"${en} handle\",\n";
}

printf "\n}";
