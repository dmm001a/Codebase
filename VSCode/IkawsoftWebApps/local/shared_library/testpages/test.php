<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Design</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    
    <style>
      :root {
  --spacing-smaller: 3px;
  --spacing-small: 5px;
  --spacing-medium: 7px;
  --spacing-large: 12px;
  --font-size: 12px;
  --font-size-large: 14px;
  --font-size-larger: 16px;
  --line-height: 16px;
  --line-height-larger: 20px;
  --primary-color: #40c979;
  --text-color-dark: #212529;
  --text-color: #585858;
  --text-color-light: #65727e;
  --border-color: #bebebe;
  --border-color-light: #f1f3f5;
  --input-placeholder: #65727e;
  --input-background: #e9e9ed;
  --input-border: #dee2e6;
  --input-border-active: #c1c9d0;
  --input-border-invalid: #e44e4e;
  --input-outline-invalid: rgba(219, 138, 138, 0.5);
  --input-color: #e9e9ed;
  --input-disabled: #f7f7f7;
  --input-min-height: 45px;
  --options-height: 40dvh;
  --option-background: #f3f4f7;
  --border-radius: 5px;
  --icon-size: 12px;
  --icon-space: 30px;
  --checkbox-size: 16px;
  --checkbox-radius: 4px;
  --checkbox-border: #ced4da;
  --checkbox-background: #fff;
  --checkbox-active: #fff;
  --checkbox-thickness: 2px;
}
.multi-select {
  display: flex;
  box-sizing: border-box;
  flex-direction: column;
  position: relative;
  width: 100%;
  user-select: none;
}
.multi-select .multi-select-header {
  border: 1px solid var(--input-border);
  border-radius: var(--border-radius);
  padding: var(--spacing-medium) var(--spacing-large);
  padding-right: var(--icon-space);
  overflow: hidden;
  gap: var(--spacing-medium);
  min-height: var(--input-min-height);
}
.multi-select .multi-select-header::after {
  content: "";
  display: block;
  position: absolute;
  top: 50%;
  right: 15px;
  transform: translateY(-50%);
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='%23949ba3' viewBox='0 0 16 16'%3E%3Cpath d='M8 13.1l-8-8 2.1-2.2 5.9 5.9 5.9-5.9 2.1 2.2z'/%3E%3C/svg%3E");
  height: var(--icon-size);
  width: var(--icon-size);
}
.multi-select .multi-select-header.multi-select-header-active {
  border-color: var(--input-border-active);
}
.multi-select .multi-select-header.multi-select-header-active::after {
  transform: translateY(-50%) rotate(180deg);
}
.multi-select .multi-select-header.multi-select-header-active + .multi-select-options {
  display: flex;
}
.multi-select .multi-select-header .multi-select-header-placeholder {
  color: var(--text-color-light);
}
.multi-select .multi-select-header .multi-select-header-option {
  display: inline-flex;
  align-items: center;
  background-color: var(--option-background);
  font-size: var(--font-size-large);
  padding: var(--spacing-smaller) var(--spacing-small);
  border-radius: var(--border-radius);
}
.multi-select .multi-select-header .multi-select-header-max {
  font-size: var(--font-size-large);
  color: var(--text-color-light);
}
.multi-select .multi-select-options {
  display: none;
  box-sizing: border-box;
  flex-flow: wrap;
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  z-index: 999;
  margin-top: var(--spacing-small);
  padding: var(--spacing-small);
  background-color: #fff;
  border-radius: var(--border-radius);
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  max-height: var(--options-height);
  overflow-y: auto;
  overflow-x: hidden;
}
.multi-select .multi-select-options::-webkit-scrollbar {
  width: 5px;
}
.multi-select .multi-select-options::-webkit-scrollbar-track {
  background: #f0f1f3;
}
.multi-select .multi-select-options::-webkit-scrollbar-thumb {
  background: #cdcfd1;
}
.multi-select .multi-select-options::-webkit-scrollbar-thumb:hover {
  background: #b2b6b9;
}
.multi-select .multi-select-options .multi-select-option,
.multi-select .multi-select-options .multi-select-all {
  padding: var(--spacing-large);
}
.multi-select .multi-select-options .multi-select-option .multi-select-option-radio,
.multi-select .multi-select-options .multi-select-all .multi-select-option-radio {
  background: var(--checkbox-background);
  margin-right: var(--spacing-large);
  height: var(--checkbox-size);
  width: var(--checkbox-size);
  border: 1px solid var(--checkbox-border);
  border-radius: var(--checkbox-radius);
}
.multi-select .multi-select-options .multi-select-option .multi-select-option-text,
.multi-select .multi-select-options .multi-select-all .multi-select-option-text {
  box-sizing: border-box;
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: inherit;
  font-size: var(--font-size-larger);
  line-height: var(--line-height);
}
.multi-select .multi-select-options .multi-select-option.multi-select-selected .multi-select-option-radio,
.multi-select .multi-select-options .multi-select-all.multi-select-selected .multi-select-option-radio {
  border-color: var(--primary-color);
  background-color: var(--primary-color);
}
.multi-select .multi-select-options .multi-select-option.multi-select-selected .multi-select-option-radio::after,
.multi-select .multi-select-options .multi-select-all.multi-select-selected .multi-select-option-radio::after {
  content: "";
  display: block;
  width: calc(var(--checkbox-size) / 4);
  height: calc(var(--checkbox-size) / 2);
  border: solid var(--checkbox-active);
  border-width: 0 var(--checkbox-thickness) var(--checkbox-thickness) 0;
  transform: rotate(45deg) translate(50%, -25%);
}
.multi-select .multi-select-options .multi-select-option.multi-select-selected .multi-select-option-text,
.multi-select .multi-select-options .multi-select-all.multi-select-selected .multi-select-option-text {
  color: var(--text-color-dark);
}
.multi-select .multi-select-options .multi-select-option:hover, .multi-select .multi-select-options .multi-select-option:active,
.multi-select .multi-select-options .multi-select-all:hover, .multi-select .multi-select-options .multi-select-all:active {
  background-color: var(--option-background);
}
.multi-select .multi-select-options .multi-select-all {
  border-bottom: 1px solid var(--border-color-light);
  border-radius: 0;
}
.multi-select .multi-select-options .multi-select-search {
  padding: var(--spacing-medium) var(--spacing-large);
  border: 1px solid var(--input-border);
  border-radius: var(--border-radius);
  margin: 10px 10px 5px 10px;
  width: 100%;
  outline: none;
  font-size: var(--font-size-larger);
}
.multi-select .multi-select-options .multi-select-search::placeholder {
  color: var(--text-color-light);
}
.multi-select .multi-select-header,
.multi-select .multi-select-option,
.multi-select .multi-select-all {
  display: flex;
  flex-wrap: wrap;
  box-sizing: border-box;
  align-items: center;
  border-radius: var(--border-radius);
  cursor: pointer;
  display: flex;
  align-items: center;
  width: 100%;
  font-size: var(--font-size-larger);
  color: var(--text-color-dark);
}
.multi-select.disabled {
  opacity: 0.6;
  pointer-events: none;
  background-color: var(--input-disabled);
}
.multi-select.multi-select-invalid .multi-select-header {
  border-color: var(--input-border-invalid);
  outline: var(--input-outline-invalid) solid 1px;
}
    </style>
 
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
    


<label for="fruits">Fruits</label>
<select id="fruits" name="fruits" data-placeholder="Select fruits" multiple data-multi-select>
    <option value="Apple">Apple</option>
    <option value="Banana">Banana</option>
    <option value="Blackberry">Blackberry</option>
    <option value="Blueberry">Blueberry</option>
    <option value="Cherry">Cherry</option>
    <option value="Cranberry">Cranberry</option>
    <option value="Grapes">Grapes</option>
    <option value="Kiwi">Kiwi</option>
    <option value="Mango">Mango</option>
    <option value="Orange">Orange</option>
    <option value="Peach">Peach</option>
    <option value="Pear">Pear</option>
    <option value="Pineapple">Pineapple</option>
    <option value="Raspberry">Raspberry</option>
    <option value="Strawberry">Strawberry</option>
    <option value="Watermelon">Watermelon</option>
</select>

   
  <script>

    /*
 * Created by David Adams
 * https://codeshack.io/multi-select-dropdown-html-javascript/
 * 
 * Released under the MIT license
 */
class MultiSelect {

    constructor(element, options = {}) {
        let defaults = {
            placeholder: 'Select item(s)',
            max: null,
            min: null,
            disabled: false,
            search: true,
            selectAll: true,
            listAll: true,
            closeListOnItemSelect: false,
            name: '',
            width: '',
            height: '',
            dropdownWidth: '',
            dropdownHeight: '',
            data: [],
            onChange: function() {},
            onSelect: function() {},
            onUnselect: function() {},
            onMaxReached: function() {}
        };
        this.options = Object.assign(defaults, options);
        this.selectElement = typeof element === 'string' ? document.querySelector(element) : element;
        this.originalSelectElement = this.selectElement.cloneNode(true);
        for(const prop in this.selectElement.dataset) {
            if (this.options[prop] !== undefined) {
                if (typeof this.options[prop] === 'boolean') {
                    this.options[prop] = this.selectElement.dataset[prop] === 'true';
                } else {
                    this.options[prop] = this.selectElement.dataset[prop];
                }
            }
        }
        this.name = this.selectElement.getAttribute('name') ? this.selectElement.getAttribute('name') : 'multi-select-' + Math.floor(Math.random() * 1000000);
        if (!this.options.data.length) {
            let options = this.selectElement.querySelectorAll('option');
            for (let i = 0; i < options.length; i++) {
                this.options.data.push({
                    value: options[i].value,
                    text: options[i].innerHTML,
                    selected: options[i].selected,
                    html: options[i].getAttribute('data-html')
                });
            }
        }
        this.originalData = JSON.parse(JSON.stringify(this.options.data));
        this.element = this._template();
        this.selectElement.replaceWith(this.element);
        this.outsideClickHandler = this._outsideClick.bind(this);
        this._updateSelected();
        this._eventHandlers();
        if (this.options.disabled) {
            this.disable();
        }
    }

    _template() {
        let optionsHTML = '';
        for (let i = 0; i < this.data.length; i++) {
            const isSelected = this.data[i].selected;
            optionsHTML += `
                <div class="multi-select-option${isSelected ? ' multi-select-selected' : ''}" data-value="${this.data[i].value}" role="option" aria-selected="${isSelected}" tabindex="-1">
                    <span class="multi-select-option-radio"></span>
                    <span class="multi-select-option-text">${this.data[i].html ? this.data[i].html : this.data[i].text}</span>
                </div>
            `;
        }
        let selectAllHTML = '';
        if (this.options.selectAll) {
            selectAllHTML = `<div class="multi-select-all" role="option" tabindex="-1">
                <span class="multi-select-option-radio"></span>
                <span class="multi-select-option-text">Select all</span>
            </div>`;
        }
        let template = `
            <div class="multi-select ${this.name}"${this.selectElement.id ? ' id="' + this.selectElement.id + '"' : ''} style="${this.width ? 'width:' + this.width + ';' : ''}${this.height ? 'height:' + this.height + ';' : ''}" role="combobox" aria-haspopup="listbox" aria-expanded="false">
                ${this.selectedValues.map(value => `<input type="hidden" name="${this.name}[]" value="${value}">`).join('')}
                <div class="multi-select-header" style="${this.width ? 'width:' + this.width + ';' : ''}${this.height ? 'height:' + this.height + ';' : ''}" tabindex="0">
                    <span class="multi-select-header-max">${this.options.max ? this.selectedValues.length + '/' + this.options.max : ''}</span>
                    <span class="multi-select-header-placeholder">${this.placeholder}</span>
                </div>
                <div class="multi-select-options" style="${this.options.dropdownWidth ? 'width:' + this.options.dropdownWidth + ';' : ''}${this.options.dropdownHeight ? 'height:' + this.options.dropdownHeight + ';' : ''}" role="listbox">
                    ${this.options.search ? '<input type="text" class="multi-select-search" placeholder="Search..." role="searchbox">' : ''}
                    ${selectAllHTML}
                    ${optionsHTML}
                </div>
            </div>
        `;
        let element = document.createElement('div');
        element.innerHTML = template;
        return element.firstElementChild;
    }

    _eventHandlers() {
        let headerElement = this.element.querySelector('.multi-select-header');
        const toggleDropdown = (forceClose = false) => {
            if (this.element.classList.contains('disabled')) return;
            if (forceClose || headerElement.classList.contains('multi-select-header-active')) {
                headerElement.classList.remove('multi-select-header-active');
                this.element.setAttribute('aria-expanded', 'false');
            } else {
                headerElement.classList.add('multi-select-header-active');
                this.element.setAttribute('aria-expanded', 'true');
            }
        };
        this.element.querySelectorAll('.multi-select-option').forEach(option => {
            option.onclick = (e) => {
                e.stopPropagation();
                if (this.element.classList.contains('disabled')) return;
                let selected = true;
                if (!option.classList.contains('multi-select-selected')) {
                    if (this.options.max && this.selectedValues.length >= this.options.max) {
                        this.options.onMaxReached(this.options.max);
                        return;
                    }
                    option.classList.add('multi-select-selected');
                    option.setAttribute('aria-selected', 'true');
                    this.element.insertAdjacentHTML('afterbegin', `<input type="hidden" name="${this.name}[]" value="${option.dataset.value}">`);
                    this.data.find(data => data.value == option.dataset.value).selected = true;
                } else {
                    option.classList.remove('multi-select-selected');
                    option.setAttribute('aria-selected', 'false');
                    this.element.querySelector(`input[value="${option.dataset.value}"]`).remove();
                    this.data.find(data => data.value == option.dataset.value).selected = false;
                    selected = false;
                }
                this._updateHeader();
                if (this.options.search) {
                    this.element.querySelector('.multi-select-search').value = '';
                    this.element.querySelectorAll('.multi-select-option').forEach(opt => opt.style.display = 'flex');
                }
                if (this.options.closeListOnItemSelect) {
                    toggleDropdown(true);
                }
                this.options.onChange(option.dataset.value, option.querySelector('.multi-select-option-text').innerHTML, option);
                if (selected) {
                    this.options.onSelect(option.dataset.value, option.querySelector('.multi-select-option-text').innerHTML, option);
                } else {
                    this.options.onUnselect(option.dataset.value, option.querySelector('.multi-select-option-text').innerHTML, option);
                }
                this._validate();
            };
        });
        headerElement.onclick = () => toggleDropdown();
        if (this.options.search) {
            let search = this.element.querySelector('.multi-select-search');
            search.oninput = () => {
                this.element.querySelectorAll('.multi-select-option').forEach(option => {
                    const text = option.querySelector('.multi-select-option-text').innerHTML.toLowerCase();
                    option.style.display = text.includes(search.value.toLowerCase()) ? 'flex' : 'none';
                });
            };
        }
        if (this.options.selectAll) {
            let selectAllButton = this.element.querySelector('.multi-select-all');
            selectAllButton.onclick = (e) => {
                e.stopPropagation();
                if (this.element.classList.contains('disabled')) return;
                let allSelected = selectAllButton.classList.contains('multi-select-selected');
                this.element.querySelectorAll('.multi-select-option').forEach(option => {
                    let dataItem = this.data.find(data => data.value == option.dataset.value);
                    if (dataItem && ((allSelected && dataItem.selected) || (!allSelected && !dataItem.selected))) {
                        option.click();
                    }
                });
                selectAllButton.classList.toggle('multi-select-selected');
            };
        }
        if (this.selectElement.id && document.querySelector('label[for="' + this.selectElement.id + '"]')) {
            document.querySelector('label[for="' + this.selectElement.id + '"]').onclick = () => {
                toggleDropdown();
            };
        }
        document.addEventListener('click', this.outsideClickHandler);
        headerElement.addEventListener('keydown', (e) => {
            if (['Enter', ' ', 'ArrowDown', 'ArrowUp'].includes(e.key)) {
                e.preventDefault();
                toggleDropdown();
                const firstElement = this.element.querySelector('[role="searchbox"]') || this.element.querySelector('[role="option"]');
                if (firstElement) firstElement.focus();
            }
        });
        this.element.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                toggleDropdown(true);
                headerElement.focus();
            }
        });
        const optionsContainer = this.element.querySelector('.multi-select-options');
        optionsContainer.addEventListener('keydown', (e) => {
            const currentFocused = document.activeElement;
            if (currentFocused.closest('.multi-select-options')) {
                if (['ArrowDown', 'ArrowUp'].includes(e.key)) {
                    e.preventDefault();
                    const direction = e.key === 'ArrowDown' ? 'nextElementSibling' : 'previousElementSibling';
                    let nextElement = currentFocused[direction];
                    while (nextElement && (nextElement.style.display === 'none' || !nextElement.matches('[role="option"], [role="searchbox"]'))) {
                        nextElement = nextElement[direction];
                    }
                    if (nextElement) nextElement.focus();
                } else if (['Enter', ' '].includes(e.key) && currentFocused.matches('[role="option"]')) {
                    e.preventDefault();
                    currentFocused.click();
                }
            }
        });
    }

    _updateHeader() {
        this.element.querySelectorAll('.multi-select-header-option, .multi-select-header-placeholder').forEach(el => el.remove());
        if (this.selectedValues.length > 0) {
            if (this.options.listAll) {
                this.selectedItems.forEach(item => {
                    const el = document.createElement('span');
                    el.className = 'multi-select-header-option';
                    el.dataset.value = item.value;
                    el.innerHTML = item.text;
                    this.element.querySelector('.multi-select-header').prepend(el);
                });
            } else {
                this.element.querySelector('.multi-select-header').insertAdjacentHTML('afterbegin', `<span class="multi-select-header-option">${this.selectedValues.length} selected</span>`);
            }
        } else {
            this.element.querySelector('.multi-select-header').insertAdjacentHTML('beforeend', `<span class="multi-select-header-placeholder">${this.placeholder}</span>`);
        }
        if (this.options.max) {
            this.element.querySelector('.multi-select-header-max').innerHTML = this.selectedValues.length + '/' + this.options.max;
        }
    }

    _updateSelected() { this._updateHeader(); }
    
    _validate() {
        if (this.options.min && this.selectedValues.length < this.options.min) {
            this.element.classList.add('multi-select-invalid');
        } else {
            this.element.classList.remove('multi-select-invalid');
        }
    }

    _outsideClick(event) {
        if (!this.element.contains(event.target) && !event.target.closest('label[for="' + this.selectElement.id + '"]')) {
            let headerElement = this.element.querySelector('.multi-select-header');
            if (headerElement.classList.contains('multi-select-header-active')) {
                headerElement.classList.remove('multi-select-header-active');
                this.element.setAttribute('aria-expanded', 'false');
            }
        }
    }

    select(value) {
        const option = this.element.querySelector(`.multi-select-option[data-value="${value}"]`);
        if (option && !option.classList.contains('multi-select-selected')) {
            option.click();
        }
    }

    unselect(value) {
        const option = this.element.querySelector(`.multi-select-option[data-value="${value}"]`);
        if (option && option.classList.contains('multi-select-selected')) {
            option.click();
        }
    }

    setValues(values) {
        this.data.forEach(item => {
            item.selected = values.includes(item.value);
        });
        this.refresh();
    }
    
    disable() {
        this.element.classList.add('disabled');
        this.element.querySelector('.multi-select-header').removeAttribute('tabindex');
        const searchInput = this.element.querySelector('.multi-select-search');
        if (searchInput) searchInput.disabled = true;
    }

    enable() {
        this.element.classList.remove('disabled');
        this.element.querySelector('.multi-select-header').setAttribute('tabindex', '0');
        const searchInput = this.element.querySelector('.multi-select-search');
        if (searchInput) searchInput.disabled = false;
    }

    destroy() {
        this.element.replaceWith(this.originalSelectElement);
        document.removeEventListener('click', this.outsideClickHandler);
    }
    
    refresh() {
        const newElement = this._template();
        this.element.replaceWith(newElement);
        this.element = newElement;
        this._updateSelected();
        this._eventHandlers();
        this._validate();
    }

    addItem(item) {
        this.options.data.push(item);
        this.refresh();
    }

    addItems(items) {
        this.options.data.push(...items);
        this.refresh();
    }

    async fetch(url, options = {}) {
        const response = await fetch(url, options);
        const data = await response.json();
        this.addItems(data);
        if (this.options.onload) {
            this.options.onload(data, this.options);
        }
    }

    removeItem(value) {
        this.options.data = this.options.data.filter(item => item.value !== value);
        this.refresh();
    }

    clear() {
        this.options.data = [];
        this.refresh();
    }

    reset() {
        this.data = JSON.parse(JSON.stringify(this.originalData));
        this.refresh();
    }

    selectAll() {
        this.data.forEach(item => item.selected = true);
        this.refresh();
    }

    get selectedValues() { return this.data.filter(d => d.selected).map(d => d.value); }
    get selectedItems() { return this.data.filter(d => d.selected); }
    get data() { return this.options.data; }
    set data(value) { this.options.data = value; }

    set selectElement(value) { this.options.selectElement = value; }
    get selectElement() { return this.options.selectElement; }

    set element(value) { this.options.element = value; }
    get element() { return this.options.element; }

    set placeholder(value) { this.options.placeholder = value; }
    get placeholder() { return this.options.placeholder; }

    set name(value) { this.options.name = value; }
    get name() { return this.options.name; }

    set width(value) { this.options.width = value; }
    get width() { return this.options.width; }

    set height(value) { this.options.height = value; }
    get height() { return this.options.height; }

}
document.querySelectorAll('[data-multi-select]').forEach(select => new MultiSelect(select));
  </script>
 </body>
</html>

